namespace VisitorLogSystem.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> CountVisitorsTodayAsync();
        Task<int> CountCurrentlyInsideAsync();
        Task<int> CountRoomVisitsTodayAsync();
        Task<string> GetTopRoomTodayAsync();
        Task<List<(DateTime Day, int Count)>> GetVisitorsPerDayAsync(int days);
        Task<(int Inside, int CheckedOut)> GetVisitorStatusAsync();
        Task<List<(string RoomName, int Count)>> GetTopRoomsAsync(int top, int days);
    }
}