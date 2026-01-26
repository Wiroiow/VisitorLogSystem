using System;

namespace VisitorLogSystem.DTOs
{
    
    /// PURPOSE: Transfer room visit data between Service and Controller layers
    /// Database → Entity (RoomVisit) → Repository → DTO → Service → DTO → Controller
     
    /// DIFFERENCE FROM VIEWMODEL:
    /// - DTO: Used between Service ↔ Controller
    /// - ViewModel: Used between Controller ↔ View (has validation, display attributes)
   
    public class RoomVisitDto
    {
        
        public int Id { get; set; }

      
        public int VisitorId { get; set; }

     
        public string? VisitorName { get; set; }

        
        public string RoomName { get; set; } = string.Empty;

        
        public DateTime EnteredAt { get; set; }

        
        public string? Purpose { get; set; }

        
        public DateTime CreatedAt { get; set; }
    }
}