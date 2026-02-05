using System;

namespace VisitorLogSystem.ViewModels
{
    
    public class VisitorBadgeViewModel
    {
        public string VisitorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public int VisitorId { get; set; }
        public int RoomVisitId { get; set; }
    }
}