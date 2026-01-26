using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IVisitorService _visitorService;
        private readonly IDashboardService _dashboardService;

        public DashboardController(IVisitorService visitorService, IDashboardService dashboardService)
        {
            _visitorService = visitorService;
            _dashboardService = dashboardService;
        }

        // YOUR EXISTING INDEX METHOD - UNCHANGED
        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalVisitorsToday = await _visitorService.GetTodayVisitorCountAsync(),
                CurrentlyInside = await _visitorService.GetCurrentlyInsideCountAsync(),
                MonthlyVisitors = await _visitorService.GetMonthlyVisitorCountAsync(),
                RecentVisitors = (await _visitorService.GetRecentVisitorsAsync(5))
                    .Select(dto => new VisitorViewModel
                    {
                        Id = dto.Id,
                        FullName = dto.FullName,
                        Purpose = dto.Purpose,
                        ContactNumber = dto.ContactNumber,
                        TimeIn = dto.TimeIn,
                        TimeOut = dto.TimeOut
                    })
                    .ToList()
            };
            return View(dashboardViewModel);
        }

        // NEW CHART ENDPOINTS
        [HttpGet]
        public async Task<IActionResult> GetVisitorsPerDay()
        {
            var data = await _dashboardService.GetVisitorsPerDayAsync(7);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetVisitorStatus()
        {
            var data = await _dashboardService.GetVisitorStatusAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopRooms()
        {
            var data = await _dashboardService.GetTopRoomsAsync(5, 30);
            return Json(data);
        }
    }
}