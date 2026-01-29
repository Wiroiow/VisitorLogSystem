using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    /// <summary>
    /// Self-Service Kiosk Controller
    /// PUBLIC ACCESS - No authentication required
    /// Reuses existing Services for all business logic
    /// </summary>
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

        /// <summary>
        /// GET: /Kiosk/Welcome
        /// Entry point for kiosk mode
        /// </summary>
        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

        #endregion

        #region Screen 2: Pre-Registration Lookup

        /// <summary>
        /// GET: /Kiosk/PreRegLookup
        /// Screen to check if visitor is pre-registered
        /// </summary>
        [HttpGet]
        public IActionResult PreRegLookup()
        {
            return View(new KioskPreRegLookupViewModel());
        }

        /// <summary>
        /// POST: /Kiosk/PreRegLookup
        /// Search for pre-registration by name
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegLookup(KioskPreRegLookupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Search for pending pre-registrations matching the name
            var preRegistrations = _preRegService.SearchPending(model.FullName);
            var matchingPreReg = preRegistrations
                .Where(pr => pr.ExpectedVisitDate.Date == DateTime.Today)
                .FirstOrDefault();

            if (matchingPreReg != null)
            {
                // Found pre-registration - pre-fill visitor details
                return RedirectToAction(nameof(VisitorDetails), new { preRegId = matchingPreReg.Id });
            }

            // Not pre-registered - proceed as walk-in visitor
            return RedirectToAction(nameof(VisitorDetails));
        }

        /// <summary>
        /// GET: /Kiosk/SkipPreReg
        /// Allow visitors to skip pre-registration lookup
        /// </summary>
        [HttpGet]
        public IActionResult SkipPreReg()
        {
            return RedirectToAction(nameof(VisitorDetails));
        }

        #endregion

        #region Screen 3: Visitor Details + Room Selection

        /// <summary>
        /// GET: /Kiosk/VisitorDetails
        /// Combined screen for visitor info and room selection
        /// </summary>
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

                    // Pre-fill room if available in PreRegisteredVisitor model
                    // Note: Your model has RoomName field but it's not being set in PreRegistrationService
                    // You may want to add this field to pre-registration form later
                }
            }

            // Populate available rooms
            ViewBag.AvailableRooms = GetAvailableRooms();

            return View(model);
        }

        /// <summary>
        /// POST: /Kiosk/VisitorDetails
        /// Process check-in
        /// </summary>
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
                    // Use existing PreRegisteredVisitorService.CheckInPreRegisteredVisitorAsync
                    // Note: This requires a userId - we'll use a system kiosk user ID
                    const int KIOSK_SYSTEM_USER_ID = 1; // You may want to create a dedicated "Kiosk" user

                    var roomVisitDto = await _preRegService.CheckInPreRegisteredVisitorAsync(
                        model.PreRegistrationId.Value,
                        KIOSK_SYSTEM_USER_ID
                    );

                    visitorId = roomVisitDto.VisitorId;
                    roomVisitId = roomVisitDto.Id;

                    // Update contact number if provided
                    if (!string.IsNullOrWhiteSpace(model.ContactNumber))
                    {
                        var visitorDto = await _visitorService.GetVisitorByIdAsync(visitorId);
                        if (visitorDto != null)
                        {
                            visitorDto.ContactNumber = model.ContactNumber;
                            await _visitorService.UpdateVisitorAsync(visitorDto);
                        }
                    }
                }
                // CASE 2: Walk-in visitor
                else
                {
                    // Create new visitor using VisitorService
                    var newVisitor = new VisitorDto
                    {
                        FullName = model.FullName,
                        Purpose = model.Purpose,
                        ContactNumber = model.ContactNumber,
                        TimeIn = DateTime.Now
                    };

                    var createdVisitor = await _visitorService.CreateVisitorAsync(newVisitor);
                    visitorId = createdVisitor.Id;

                    // Create room visit using RoomVisitService
                    var roomVisitDto = await _roomVisitService.RecordRoomEntryAsync(
                        visitorId,
                        model.RoomName,
                        model.Purpose
                    );

                    roomVisitId = roomVisitDto.Id;
                }

                // Redirect to success screen
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

        /// <summary>
        /// GET: /Kiosk/Success
        /// Show check-in confirmation
        /// </summary>
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

        /// <summary>
        /// Get list of available rooms for dropdown
        /// TODO: Move this to a configuration file or database table
        /// </summary>
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