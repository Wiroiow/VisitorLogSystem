using System;

namespace VisitorLogSystem.DTOs
{
    public class PreRegisteredVisitorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Purpose { get; set; }
        public DateTime ExpectedVisitDate { get; set; }
        public int HostUserId { get; set; }
        public string HostUserName { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CheckedInByUserId { get; set; }
        public string CheckedInByUserName { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public int? RoomVisitId { get; set; }
    }
}