using System;

namespace VisitorLogSystem.DTOs
{
    
    public class VisitorDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty; 

        public string Purpose { get; set; } = string.Empty; 

        public string? ContactNumber { get; set; } 

        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}