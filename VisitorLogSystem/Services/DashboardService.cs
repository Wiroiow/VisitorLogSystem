using System.Globalization;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;

namespace VisitorLogSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var summary = new DashboardSummaryDto
            {
                TotalVisitorsToday = await _dashboardRepository.CountVisitorsTodayAsync(),
                CurrentlyInside = await _dashboardRepository.CountCurrentlyInsideAsync(),
                TotalRoomVisitsToday = await _dashboardRepository.CountRoomVisitsTodayAsync(),
                TopRoomToday = await _dashboardRepository.GetTopRoomTodayAsync()
            };

            return summary;
        }

        public async Task<ChartDataDto> GetVisitorsPerDayAsync(int days)
        {
            var data = await _dashboardRepository.GetVisitorsPerDayAsync(days);

            var chartData = new ChartDataDto
            {
                Labels = data.Select(x => x.Day.ToString("yyyy-MM-dd")).ToList(),
                Data = data.Select(x => x.Count).ToList()
            };

            return chartData;
        }

        public async Task<ChartDataDto> GetVisitorStatusAsync()
        {
            var status = await _dashboardRepository.GetVisitorStatusAsync();

            var chartData = new ChartDataDto
            {
                Labels = new List<string> { "Inside", "Checked Out" },
                Data = new List<int> { status.Inside, status.CheckedOut }
            };

            return chartData;
        }

        public async Task<ChartDataDto> GetTopRoomsAsync(int top, int days)
        {
            var rooms = await _dashboardRepository.GetTopRoomsAsync(top, days);

            var chartData = new ChartDataDto
            {
                Labels = rooms.Select(x => x.RoomName).ToList(),
                Data = rooms.Select(x => x.Count).ToList()
            };

            return chartData;
        }
    }
}