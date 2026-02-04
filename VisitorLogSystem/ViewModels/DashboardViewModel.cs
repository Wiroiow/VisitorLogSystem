using System.Collections.Generic;

namespace VisitorLogSystem.ViewModels
{
    /// Contains all statistics and data needed for the dashboard  
    public class DashboardViewModel
    {
        public int TotalVisitorsToday { get; set; }
        public int CurrentlyInside { get; set; }
        public int MonthlyVisitors { get; set; }

        public int TotalRoomVisitsToday { get; set; }

        public List<VisitorViewModel> RecentVisitors { get; set; }

        // Latest Room Visits (for dashboard display)
        public List<RoomVisitViewModel> RecentRoomVisits { get; set; }

        public DashboardViewModel()
        {
            RecentVisitors = new List<VisitorViewModel>();
            RecentRoomVisits = new List<RoomVisitViewModel>();
        }
    }

    // Helper ViewModel for displaying room visits on dashboard
    public class RoomVisitViewModel
    {
        public int Id { get; set; }
        public string VisitorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public System.DateTime EnteredAt { get; set; }
        public string? Purpose { get; set; }
        public bool VisitorSignedOut { get; set; }
    }
}