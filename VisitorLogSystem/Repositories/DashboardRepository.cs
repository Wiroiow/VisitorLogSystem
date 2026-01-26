using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;

namespace VisitorLogSystem.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountVisitorsTodayAsync()
        {
            var todayStart = DateTime.Today;
            var tomorrowStart = DateTime.Today.AddDays(1);

            return await _context.Visitors
                .Where(v => v.TimeIn >= todayStart && v.TimeIn < tomorrowStart)
                .CountAsync();
        }

        public async Task<int> CountCurrentlyInsideAsync()
        {
            return await _context.Visitors
                .Where(v => v.TimeOut == null)
                .CountAsync();
        }

        public async Task<int> CountRoomVisitsTodayAsync()
        {
            var todayStart = DateTime.Today;
            var tomorrowStart = DateTime.Today.AddDays(1);

            return await _context.RoomVisits
                .Where(rv => rv.EnteredAt >= todayStart && rv.EnteredAt < tomorrowStart)
                .CountAsync();
        }

        public async Task<string> GetTopRoomTodayAsync()
        {
            var todayStart = DateTime.Today;
            var tomorrowStart = DateTime.Today.AddDays(1);

            var topRoom = await _context.RoomVisits
                .Where(rv => rv.EnteredAt >= todayStart && rv.EnteredAt < tomorrowStart)
                .GroupBy(rv => rv.RoomName)
                .Select(g => new { RoomName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            return topRoom?.RoomName ?? "N/A";
        }

        public async Task<List<(DateTime Day, int Count)>> GetVisitorsPerDayAsync(int days)
        {
            var startDate = DateTime.Today.AddDays(-days + 1);
            var endDate = DateTime.Today.AddDays(1);

            var visitors = await _context.Visitors
                .Where(v => v.TimeIn >= startDate && v.TimeIn < endDate)
                .Select(v => v.TimeIn)
                .ToListAsync();

            // Group in memory to avoid SQL translation issues
            var grouped = visitors
                .GroupBy(timeIn => timeIn.Date)
                .Select(g => (Day: g.Key, Count: g.Count()))
                .OrderBy(x => x.Day)
                .ToList();

            // Ensure all days are represented (fill missing days with 0)
            var result = new List<(DateTime Day, int Count)>();
            for (int i = 0; i < days; i++)
            {
                var day = DateTime.Today.AddDays(-days + 1 + i);
                var count = grouped.FirstOrDefault(x => x.Day == day).Count;
                result.Add((day, count));
            }

            return result;
        }

        public async Task<(int Inside, int CheckedOut)> GetVisitorStatusAsync()
        {
            var inside = await _context.Visitors
                .Where(v => v.TimeOut == null)
                .CountAsync();

            var checkedOut = await _context.Visitors
                .Where(v => v.TimeOut != null)
                .CountAsync();

            return (inside, checkedOut);
        }

        public async Task<List<(string RoomName, int Count)>> GetTopRoomsAsync(int top, int days)
        {
            var startDate = DateTime.Today.AddDays(-days);
            var endDate = DateTime.Today.AddDays(1);

            var topRooms = await _context.RoomVisits
                .Where(rv => rv.EnteredAt >= startDate && rv.EnteredAt < endDate)
                .GroupBy(rv => rv.RoomName)
                .Select(g => new { RoomName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToListAsync();

            return topRooms.Select(x => (x.RoomName, x.Count)).ToList();
        }
    }
}