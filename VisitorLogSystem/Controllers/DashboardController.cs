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

        public DashboardController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        public async Task<IActionResult> Index()
        {
            // You can now access user info from claims!
            // User.Identity.Name = "admin"
            // User.FindFirst(ClaimTypes.NameIdentifier).Value = "1"
            // User.IsInRole("Admin") = true

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
    }
}