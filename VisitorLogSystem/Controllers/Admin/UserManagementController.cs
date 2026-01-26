using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels.Admin;

namespace VisitorLogSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Users")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userDtos = await _userManagementService.GetAllUsersAsync();

            var viewModels = userDtos.Select(dto => new UserViewModel
            {
                Id = dto.Id,
                Username = dto.Username,
                Role = dto.Role,
                CreatedAt = dto.CreatedAt
            }).ToList();

           
            return View("~/Views/Admin/UserManagement/Index.cshtml", viewModels);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            // ✅ Specify exact view path
            return View("~/Views/Admin/UserManagement/Create.cshtml", new CreateUserViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/UserManagement/Create.cshtml", model);
            }

            if (await _userManagementService.UsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View("~/Views/Admin/UserManagement/Create.cshtml", model);
            }

            await _userManagementService.CreateUserAsync(
                model.Username,
                model.Password,
                model.Role
            );

            TempData["Success"] = $"User '{model.Username}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var userDto = await _userManagementService.GetUserByIdAsync(id);

            if (userDto == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Role = userDto.Role,
                CreatedAt = userDto.CreatedAt
            };

            return View("~/Views/Admin/UserManagement/Edit.cshtml", viewModel);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/UserManagement/Edit.cshtml", model);
            }

            if (await _userManagementService.UsernameExistsAsync(model.Username, id))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View("~/Views/Admin/UserManagement/Edit.cshtml", model);
            }

            var result = await _userManagementService.UpdateUserAsync(
                id,
                model.Username,
                model.NewPassword,
                model.Role
            );

            if (result == null)
            {
                TempData["Error"] = "Failed to update user.";
                return View("~/Views/Admin/UserManagement/Edit.cshtml", model);
            }

            TempData["Success"] = $"User '{model.Username}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userDto = await _userManagementService.GetUserByIdAsync(id);

            if (userDto == null)
            {
                return NotFound();
            }

            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserId = int.TryParse(currentUserIdClaim, out var userId) ? userId : 0;

            if (id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new UserViewModel
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Role = userDto.Role,
                CreatedAt = userDto.CreatedAt
            };

            return View("~/Views/Admin/UserManagement/Delete.cshtml", viewModel);
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserId = int.TryParse(currentUserIdClaim, out var userId) ? userId : 0;

            var result = await _userManagementService.DeleteUserAsync(id, currentUserId);

            if (!result)
            {
                TempData["Error"] = "Failed to delete user. You cannot delete your own account.";
            }
            else
            {
                TempData["Success"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}