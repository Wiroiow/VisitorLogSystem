using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;
using VisitorLogSystem.ViewModels.Admin;

namespace VisitorLogSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IVisitorService _visitorService;

        public AdminDashboardController(
            IUserManagementService userManagementService,
            IVisitorService visitorService)
        {
            _userManagementService = userManagementService;
            _visitorService = visitorService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var currentUsername = User.Identity?.Name ?? "Unknown";

            var (totalUsers, admins, staff) = await _userManagementService.GetUserStatisticsAsync();

            var allVisitors = await _visitorService.GetAllVisitorsAsync();
            var totalVisitors = allVisitors.Count;
            var visitorsToday = await _visitorService.GetTodayVisitorCountAsync();
            var visitorsThisMonth = await _visitorService.GetMonthlyVisitorCountAsync();

            var recentUsers = (await _userManagementService.GetAllUsersAsync())
                .Take(5)
                .Select(dto => new UserViewModel
                {
                    Id = dto.Id,
                    Username = dto.Username,
                    Role = dto.Role,
                    CreatedAt = dto.CreatedAt
                })
                .ToList();

            var recentActivity = (await _visitorService.GetRecentVisitorsAsync(5))
                .Select(dto => new VisitorViewModel
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Purpose = dto.Purpose,
                    ContactNumber = dto.ContactNumber,
                    TimeIn = dto.TimeIn,
                    TimeOut = dto.TimeOut
                })
                .ToList();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalAdmins = admins,
                TotalStaff = staff,
                TotalVisitors = totalVisitors,
                VisitorsToday = visitorsToday,
                VisitorsThisMonth = visitorsThisMonth,
                RecentUsers = recentUsers,
                RecentActivity = recentActivity
            };

            ViewBag.CurrentUsername = currentUsername;

            // ✅ Specify exact view path
            return View("~/Views/Admin/AdminDashboard/Index.cshtml", viewModel);
        }
    }
}
