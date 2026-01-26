using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<ChartDataDto> GetVisitorsPerDayAsync(int days);
        Task<ChartDataDto> GetVisitorStatusAsync();
        Task<ChartDataDto> GetTopRoomsAsync(int top, int days);
    }
}