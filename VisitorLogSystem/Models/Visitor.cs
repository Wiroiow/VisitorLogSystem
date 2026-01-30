using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VisitorLogSystem.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Purpose { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RoomVisit>? RoomVisits { get; set; }

        
        public string DisplayContactNumber()
        {
            return string.IsNullOrWhiteSpace(ContactNumber) ? "Not provided" : ContactNumber;
        }

       
        public bool IsCurrentlyInBuilding()
        {
            return !TimeOut.HasValue;
        }

        
        public int RoomVisitCount()
        {
            return RoomVisits?.Count ?? 0;
        }

        
        public string GetDuration()
        {
            if (!TimeOut.HasValue)
                return "Still inside";

            var duration = TimeOut.Value - TimeIn;
            return $"{duration.Hours}h {duration.Minutes}m";
        }

       
        public string GetStatus()
        {
            return TimeOut.HasValue ? "Checked Out" : "Inside";
        }
    }
}