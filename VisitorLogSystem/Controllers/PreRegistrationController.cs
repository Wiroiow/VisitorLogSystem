using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    [Authorize]
    public class PreRegistrationController : Controller
    {
        private readonly IPreRegisteredVisitorService _preRegService;
        private readonly IUserManagementService _userService;
        private readonly IAuthService _authService;

        public PreRegistrationController(
            IPreRegisteredVisitorService preRegService,
            IUserManagementService userService,
            IAuthService authService)
        {
            _preRegService = preRegService;
            _userService = userService;
            _authService = authService;
        }

        
        public IActionResult Index(string searchTerm, DateTime? filterDate, bool showOnlyPending = true)
        {
            var currentUser = _authService.GetCurrentUserId();
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            var preRegistrations = showOnlyPending
                ? _preRegService.GetPendingVisitors().ToList()
                : _preRegService.GetAllPreRegisteredVisitors().ToList();

            // FIX Extract .Value from nullable DateTime
            if (filterDate.HasValue)
            {
                preRegistrations = _preRegService.GetPendingVisitorsByDate(filterDate.Value).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm) && showOnlyPending)
            {
                preRegistrations = _preRegService.SearchPending(searchTerm).ToList();
            }

            var viewModel = new PreRegistrationListViewModel
            {
                PreRegistrations = preRegistrations.Select(dto => new PreRegistrationViewModel
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Purpose = dto.Purpose,
                    ExpectedVisitDate = dto.ExpectedVisitDate,
                    HostUserId = dto.HostUserId,
                    HostUserName = dto.HostUserName,
                    IsCheckedIn = dto.IsCheckedIn,
                    CreatedAt = dto.CreatedAt,
                    CheckedInByUserName = dto.CheckedInByUserName,
                    CheckedInAt = dto.CheckedInAt ?? default(DateTime), 
                    RoomVisitId = dto.RoomVisitId
                }).ToList(),
                SearchTerm = searchTerm,
                FilterDate = filterDate,
                ShowOnlyPending = showOnlyPending
            };

            return View(viewModel);
        }

        // GET: PreRegistration/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = _authService.GetCurrentUserId();
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            var hosts = await _userService.GetAllUsersAsync();

            var viewModel = new PreRegistrationViewModel
            {
                ExpectedVisitDate = DateTime.Now.AddDays(1),
                HostUserId = currentUser,
                AvailableHosts = hosts.ToList()
            };

            return View(viewModel);
        }

        // POST: PreRegistration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PreRegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var hosts = await _userService.GetAllUsersAsync();
                viewModel.AvailableHosts = hosts.ToList();
                return View(viewModel);
            }

            try
            {
                var dto = new PreRegisteredVisitorDto
                {
                    FullName = viewModel.FullName,
                    Purpose = viewModel.Purpose,
                    ExpectedVisitDate = viewModel.ExpectedVisitDate,
                    HostUserId = viewModel.HostUserId
                };

                _preRegService.CreatePreRegistration(dto);
                TempData["SuccessMessage"] = "Visitor pre-registered successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating pre-registration: " + ex.Message);
                var hosts = await _userService.GetAllUsersAsync();
                viewModel.AvailableHosts = hosts.ToList();
                return View(viewModel);
            }
        }

        // GET: PreRegistration/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = _preRegService.GetById(id);
            if (dto == null)
            {
                return NotFound();
            }

            if (dto.IsCheckedIn)
            {
                TempData["ErrorMessage"] = "Cannot edit a pre-registration that has been checked in.";
                return RedirectToAction("Index");
            }

            var hosts = await _userService.GetAllUsersAsync();

            var viewModel = new PreRegistrationViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Purpose = dto.Purpose,
                ExpectedVisitDate = dto.ExpectedVisitDate,
                HostUserId = dto.HostUserId,
                AvailableHosts = hosts.ToList()
            };

            return View(viewModel);
        }

        // POST: PreRegistration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PreRegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var hosts = await _userService.GetAllUsersAsync();
                viewModel.AvailableHosts = hosts.ToList();
                return View(viewModel);
            }

            try
            {
                var dto = new PreRegisteredVisitorDto
                {
                    Id = viewModel.Id,
                    FullName = viewModel.FullName,
                    Purpose = viewModel.Purpose,
                    ExpectedVisitDate = viewModel.ExpectedVisitDate,
                    HostUserId = viewModel.HostUserId
                };

                _preRegService.UpdatePreRegistration(dto);
                TempData["SuccessMessage"] = "Pre-registration updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating pre-registration: " + ex.Message);
                var hosts = await _userService.GetAllUsersAsync();
                viewModel.AvailableHosts = hosts.ToList();
                return View(viewModel);
            }
        }

        // POST: PreRegistration/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _preRegService.DeletePreRegistration(id);
                TempData["SuccessMessage"] = "Pre-registration deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting pre-registration: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: PreRegistration/CheckIn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var currentUser = _authService.GetCurrentUserId();
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                //FIX: Pass default room name (or get from form if available)
                var roomVisit = await _preRegService.CheckInPreRegisteredVisitorAsync(id, currentUser, "Main Office");
                TempData["SuccessMessage"] = $"Visitor checked in successfully! Visit ID: {roomVisit.Id}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error checking in visitor: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: PreRegistration/Details/5
        public IActionResult Details(int id)
        {
            var dto = _preRegService.GetById(id);
            if (dto == null)
            {
                return NotFound();
            }

            var viewModel = new PreRegistrationViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Purpose = dto.Purpose,
                ExpectedVisitDate = dto.ExpectedVisitDate,
                HostUserId = dto.HostUserId,
                HostUserName = dto.HostUserName,
                IsCheckedIn = dto.IsCheckedIn,
                CreatedAt = dto.CreatedAt,
                CheckedInByUserName = dto.CheckedInByUserName,
                CheckedInAt = dto.CheckedInAt ?? default(DateTime), 
                RoomVisitId = dto.RoomVisitId
            };

            return View(viewModel);
        }
    }
}