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

            //Proper case-insensitive search with strict null checking
            var preRegistrations = _preRegService.SearchPending(model.FullName);
            var matchingPreReg = preRegistrations
                .Where(pr => pr.ExpectedVisitDate.Date == DateTime.Today)
                .FirstOrDefault();

            //Only continue if found, otherwise show not found message
            if (matchingPreReg != null)
            {
                // Found - redirect with preRegId
                return RedirectToAction(nameof(VisitorDetails), new { preRegId = matchingPreReg.Id });
            }

            //Not found - set error message and return to lookup view
            TempData["ErrorMessage"] = $"No pre-registration found for '{model.FullName}' today. Please use Walk-In registration.";
            return View(model);
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

            //Strict validation before using pre-registration data
            if (preRegId.HasValue)
            {
                var preReg = _preRegService.GetById(preRegId.Value);

                //Only proceed if record exists AND not already checked in
                if (preReg != null && !preReg.IsCheckedIn)
                {
                    model.PreRegistrationId = preReg.Id;
                    model.IsPreRegistered = true;
                    model.FullName = preReg.FullName;
                    model.PreRegPurpose = preReg.Purpose;
                    model.Purpose = preReg.Purpose ?? string.Empty;
                    model.HostUserId = preReg.HostUserId;
                    model.PreRegRoomName = preReg.RoomName; // Use pre-registered room if available

                    //Pre-select room if it was specified
                    if (!string.IsNullOrWhiteSpace(preReg.RoomName))
                    {
                        model.RoomName = preReg.RoomName;
                    }
                }
                else
                {
                    //Invalid or already checked in - redirect to walk-in
                    TempData["ErrorMessage"] = "Pre-registration not found or already checked in.";
                    return RedirectToAction(nameof(VisitorDetails)); // No preRegId
                }
            }

            ViewBag.AvailableRooms = GetAvailableRooms();
            return View(model);
        }

       
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

                    // Pass room name to check-in method
                    var roomVisitDto = await _preRegService.CheckInPreRegisteredVisitorAsync(
                        model.PreRegistrationId.Value,
                        KIOSK_SYSTEM_USER_ID,
                        model.RoomName //Pass selected room
                    );

                    visitorId = roomVisitDto.VisitorId;
                    roomVisitId = roomVisitDto.Id;

                    // Update contact info if provided
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
                    //FIX: Ensure TimeIn is set explicitly
                    var visitorDto = new VisitorDto
                    {
                        FullName = model.FullName,
                        Purpose = model.Purpose,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email,
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