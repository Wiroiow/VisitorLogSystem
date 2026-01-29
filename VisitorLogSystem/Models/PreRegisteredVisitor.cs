using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLogSystem.Models
{
    public class PreRegisteredVisitor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        public string Purpose { get; set; } = string.Empty;

        [Required]
        public DateTime ExpectedVisitDate { get; set; }

        [Required]
        public int HostUserId { get; set; }

        [ForeignKey("HostUserId")]
        public User? HostUser { get; set; } 

        public bool IsCheckedIn { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public int? CheckedInByUserId { get; set; }

        [ForeignKey("CheckedInByUserId")]
        public User? CheckedInByUser { get; set; }

        public DateTime CheckedInAt { get; set; }

        
        public int? RoomVisitId { get; set; }

        [ForeignKey("RoomVisitId")]
        public RoomVisit? RoomVisit { get; set; }

        [MaxLength(100)]
        public string? RoomName { get; set; }
    }
}