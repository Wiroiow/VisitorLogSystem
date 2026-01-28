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
        public required User HostUser { get; set; }

        public bool IsCheckedIn { get; set; }

        public DateTime CreatedAt { get; set; }

        // Optional: Track who actually checked them in
        public int? CheckedInByUserId { get; set; }

        [ForeignKey("CheckedInByUserId")]
        public required User CheckedInByUser { get; set; }

        public DateTime? CheckedInAt { get; set; }

        // Optional: Link to the created RoomVisit
        public int? RoomVisitId { get; set; }

        [ForeignKey("RoomVisitId")]
        public required RoomVisit RoomVisit { get; set; }

        [MaxLength(100)]
        public string? RoomName { get; set; }
    }
}