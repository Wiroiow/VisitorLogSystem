using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IEmailService _emailService;
        private readonly IUserManagementService _userService;

        public KioskController(
            IVisitorService visitorService,
            IRoomVisitService roomVisitService,
            IPreRegisteredVisitorService preRegService,
            IEmailService emailService,
            IUserManagementService userService)
        {
            _visitorService = visitorService;
            _roomVisitService = roomVisitService;
            _preRegService = preRegService;
            _emailService = emailService;
            _userService = userService;
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

            TempData["ErrorMessage"] = $"No pre-registration found for '{model.FullName}' today. Please use Walk-In registration.";
            return View(model);
        }

        //QR Code Scan Lookup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QRCodeLookup(string qrCodeValue)
        {
            if (string.IsNullOrWhiteSpace(qrCodeValue))
            {
                TempData["ErrorMessage"] = "Invalid QR code scanned.";
                return RedirectToAction(nameof(PreRegLookup));
            }

            var preReg = _preRegService.GetByQRCode(qrCodeValue);

            if (preReg != null && !preReg.IsCheckedIn)
            {
                return RedirectToAction(nameof(VisitorDetails), new { preRegId = preReg.Id });
            }

            TempData["ErrorMessage"] = "QR code not found or visitor already checked in.";
            return RedirectToAction(nameof(PreRegLookup));
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

                    if (!string.IsNullOrWhiteSpace(preReg.RoomName))
                    {
                        model.RoomName = preReg.RoomName;
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Pre-registration not found or already checked in.";
                    return RedirectToAction(nameof(VisitorDetails));
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

                    var roomVisitDto = await _preRegService.CheckInPreRegisteredVisitorAsync(
                        model.PreRegistrationId.Value,
                        KIOSK_SYSTEM_USER_ID,
                        model.RoomName
                    );

                    visitorId = roomVisitDto.VisitorId;
                    roomVisitId = roomVisitDto.Id;

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
                // CASE 2: Walk-in visitor
                else
                {
                    var visitorDto = new VisitorDto
                    {
                        FullName = model.FullName,
                        Purpose = model.Purpose,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email,
                        TimeIn = DateTime.Now
                    };

                    var visitor = await _visitorService.FindOrCreateVisitorAsync(visitorDto);
                    visitorId = visitor.Id;

                    var roomVisit = await _roomVisitService.RecordRoomEntryAsync(
                        visitorId,
                        model.RoomName,
                        model.Purpose
                    );
                    roomVisitId = roomVisit.Id;
                }

                
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.Email))
                    {
                        await _emailService.SendVisitorConfirmationEmailAsync(
                            model.Email,
                            model.FullName,
                            model.RoomName,
                            model.Purpose,
                            DateTime.Now
                        );
                    }

                    if (model.IsPreRegistered && model.HostUserId.HasValue)
                    {
                        var preReg = _preRegService.GetById(model.PreRegistrationId!.Value);
                        if (preReg != null)
                        {
                            var allUsers = await _userService.GetAllUsersAsync();
                            var host = allUsers.FirstOrDefault(u => u.Id == preReg.HostUserId);

                            if (host?.Email != null)
                            {
                                await _emailService.SendVisitorArrivalNotificationAsync(
                                    host.Email,
                                    host.Username ?? "Host",
                                    model.FullName,
                                    model.Purpose,
                                    model.RoomName,
                                    DateTime.Now
                                );
                            }
                        }
                    }
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Email notification failed: {emailEx.Message}");
                }

                
                return RedirectToAction(nameof(Badge), new
                {
                    visitorId = visitorId,
                    roomVisitId = roomVisitId
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

        #region ✅ NEW: Badge Generation

        [HttpGet]
        public async Task<IActionResult> Badge(int visitorId, int roomVisitId)
        {
            try
            {
                var visitor = await _visitorService.GetVisitorByIdAsync(visitorId);
                var roomVisit = await _roomVisitService.GetVisitorRoomHistoryAsync(visitorId);
                var currentVisit = roomVisit.FirstOrDefault(rv => rv.Id == roomVisitId);

                if (visitor == null || currentVisit == null)
                {
                    TempData["ErrorMessage"] = "Visitor or visit details not found.";
                    return RedirectToAction(nameof(Welcome));
                }

                var badgeModel = new VisitorBadgeViewModel
                {
                    VisitorName = visitor.FullName,
                    RoomName = currentVisit.Purpose, // Note: Your DTO maps RoomName to Purpose
                    VisitDate = currentVisit.TimeIn,
                    Purpose = visitor.Purpose,
                    ContactNumber = visitor.ContactNumber,
                    VisitorId = visitorId,
                    RoomVisitId = roomVisitId
                };

                return View(badgeModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating badge: {ex.Message}";
                return RedirectToAction(nameof(Welcome));
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