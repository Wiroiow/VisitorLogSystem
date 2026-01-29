using System;

namespace VisitorLogSystem.DTOs
{
    public class RoomVisitDto
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public string FullName { get; set; }  = string.Empty;      
        public string VisitorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime TimeIn { get; set; }        
        public DateTime EnteredAt { get; set; }     
        public DateTime? TimeOut { get; set; }
        public DateTime CreatedAt { get; set; }    
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}