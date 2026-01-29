using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    
    public class KioskController : Controller
    {
        private readonly IVisitorService _visitorService;
        private readonly IRoomVisitService _roomVisitService;
        private readonly IPreRegisteredVisitorService _preRegService;

        public KioskController(
            IVisitorService visitorService,
            IRoomVisitService roomVisitService,
            IPreRegisteredVisitorService preRegService)
        {
            _visitorService = visitorService;
            _roomVisitService = roomVisitService;
            _preRegService = preRegService;
        }

        #region Screen 1: Welcome Screen

      
        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

        #endregion

        #region Screen 2: Pre-Registration Lookup

      
        [HttpGet]
        public IActionResult PreRegLookup()
        {
            return View(new KioskPreRegLookupViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegLookup(KioskPreRegLookupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            var preRegistrations = _preRegService.SearchPending(model.FullName);
            var matchingPreReg = preRegistrations
                .Where(pr => pr.ExpectedVisitDate.Date == DateTime.Today)
                .FirstOrDefault();

            if (matchingPreReg != null)
            {
                
                return RedirectToAction(nameof(VisitorDetails), new { preRegId = matchingPreReg.Id });
            }

            
            return RedirectToAction(nameof(VisitorDetails));
        }

        
        [HttpGet]
        public IActionResult SkipPreReg()
        {
            return RedirectToAction(nameof(VisitorDetails));
        }

        #endregion

        #region Screen 3: Visitor Details + Room Selection

        
        [HttpGet]
        public IActionResult VisitorDetails(int? preRegId)
        {
            var model = new KioskCheckInViewModel();

            // If pre-registered, pre-fill the form
            if (preRegId.HasValue)
            {
                var preReg = _preRegService.GetById(preRegId.Value);
                if (preReg != null && !preReg.IsCheckedIn)
                {
                    model.PreRegistrationId = preReg.Id;
                    model.IsPreRegistered = true;
                    model.FullName = preReg.FullName;
                    model.PreRegPurpose = preReg.Purpose;
                    model.Purpose = preReg.Purpose ?? string.Empty;
                    model.HostUserId = preReg.HostUserId;

                   
                }
            }

           
            ViewBag.AvailableRooms = GetAvailableRooms();

            return View(model);
        }

       
        /// Process check-in with duplicate detection
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VisitorDetails(KioskCheckInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AvailableRooms = GetAvailableRooms();
                return View(model);
            }

            try
            {
                int visitorId;
                int roomVisitId;

                // CASE 1: Pre-registered visitor
                if (model.IsPreRegistered && model.PreRegistrationId.HasValue)
                {
                    const int KIOSK_SYSTEM_USER_ID = 1; 

                    var roomVisitDto = await _preRegService.CheckInPreRegisteredVisitorAsync(
                        model.PreRegistrationId.Value,
                        KIOSK_SYSTEM_USER_ID
                    );

                    visitorId = roomVisitDto.VisitorId;
                    roomVisitId = roomVisitDto.Id;

                    //Update contact info if provided
                    var visitorDto = await _visitorService.GetVisitorByIdAsync(visitorId);
                    if (visitorDto != null)
                    {
                        bool needsUpdate = false;

                        if (!string.IsNullOrWhiteSpace(model.ContactNumber))
                        {
                            visitorDto.ContactNumber = model.ContactNumber;
                            needsUpdate = true;
                        }

                        if (!string.IsNullOrWhiteSpace(model.Email))
                        {
                            visitorDto.Email = model.Email;
                            needsUpdate = true;
                        }

                        if (needsUpdate)
                        {
                            await _visitorService.UpdateVisitorAsync(visitorDto);
                        }
                    }
                }
                // CASE 2: Walk-in visitor (with duplicate detection)
                else
                {
                    //Use FindOrCreateVisitor to prevent duplicates
                    var visitorDto = new VisitorDto
                    {
                        FullName = model.FullName,
                        Purpose = model.Purpose,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email, // ✅ NEW
                        TimeIn = DateTime.Now
                    };

                    // This will find existing visitor by email or create new
                    var visitor = await _visitorService.FindOrCreateVisitorAsync(visitorDto);
                    visitorId = visitor.Id;

                   
                    var roomVisitDto = await _roomVisitService.RecordRoomEntryAsync(
                        visitorId,
                        model.RoomName,
                        model.Purpose
                    );

                    roomVisitId = roomVisitDto.Id;
                }

                
                return RedirectToAction(nameof(Success), new
                {
                    name = model.FullName,
                    room = model.RoomName,
                    purpose = model.Purpose,
                    wasPreReg = model.IsPreRegistered
                });
            }
            catch (Exception ex)
            {
                // Log error (you may want to inject ILogger here)
                ModelState.AddModelError("", $"Check-in failed: {ex.Message}");
                ViewBag.AvailableRooms = GetAvailableRooms();
                return View(model);
            }
        }
        #endregion

        #region Screen 4: Success Confirmation

        
        
        [HttpGet]
        public IActionResult Success(string name, string room, string purpose, bool wasPreReg = false)
        {
            var model = new KioskSuccessViewModel
            {
                FullName = name,
                RoomName = room,
                Purpose = purpose,
                CheckInTime = DateTime.Now,
                WasPreRegistered = wasPreReg
            };

            return View(model);
        }

        #endregion

        #region Helper Methods

       
        private string[] GetAvailableRooms()
        {
            return new[]
            {
                "Main Office",
                "Conference Room A",
                "Conference Room B",
                "Meeting Room 1",
                "Meeting Room 2",
                "Reception Area",
                "Training Room",
                "Executive Suite",
                "IT Department",
                "HR Department"
            };
        }

        #endregion
    }
}