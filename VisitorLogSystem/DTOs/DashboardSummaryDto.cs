namespace VisitorLogSystem.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalVisitorsToday { get; set; }
        public int CurrentlyInside { get; set; }
        public int TotalRoomVisitsToday { get; set; }
        public string TopRoomToday { get; set; } = "N/A";
    }
}