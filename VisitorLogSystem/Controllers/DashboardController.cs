using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IRoomVisitService _roomVisitService;

        public DashboardController(
            IVisitorService visitorService,
            IDashboardService dashboardService,
            IRoomVisitService roomVisitService)
        {
            _visitorService = visitorService;
            _dashboardService = dashboardService;
            _roomVisitService = roomVisitService;
        }

        // Added TotalRoomVisitsToday calculation
        public async Task<IActionResult> Index()
        {
            // Get latest 10 room visits for dashboard table
            var latestRoomVisits = await _roomVisitService.GetLatestRoomVisitsAsync(10);

            //Get today's room visits count
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayRoomVisits = await _roomVisitService.GetRoomVisitsByDateRangeAsync(today, tomorrow);

            var dashboardViewModel = new DashboardViewModel
            {
                TotalVisitorsToday = await _visitorService.GetTodayVisitorCountAsync(),
                CurrentlyInside = await _visitorService.GetCurrentlyInsideCountAsync(),
                MonthlyVisitors = await _visitorService.GetMonthlyVisitorCountAsync(),

                //Set today's room visits count
                TotalRoomVisitsToday = todayRoomVisits.Count,

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
                    .ToList(),

                RecentRoomVisits = latestRoomVisits
                    .Select(dto => new RoomVisitViewModel
                    {
                        Id = dto.Id,
                        VisitorName = dto.FullName,
                        RoomName = dto.Purpose,
                        EnteredAt = dto.TimeIn,
                        Purpose = dto.Purpose,
                        VisitorSignedOut = dto.TimeOut.HasValue
                    })
                    .ToList()
            };

            return View(dashboardViewModel);
        }

        // Chart endpoints 
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