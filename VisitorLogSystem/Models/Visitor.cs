using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLogSystem.Models
{
    [Table("visitors")]
    public class Visitor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("full_name")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("purpose")]
        [MaxLength(500)]
        public string Purpose { get; set; } = string.Empty;

        [Column("contact_number")]
        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [Required]
        [Column("time_in")]
        public DateTime TimeIn { get; set; }

        [Column("time_out")]
        public DateTime? TimeOut { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

      
        /// This represents ALL room visits made by this visitor
        /// Entity Framework uses this to establish the one-to-many relationship
       
        public virtual ICollection<RoomVisit> RoomVisits { get; set; } = new List<RoomVisit>();

       
        /// Helper property to check if visitor is still in the building
        
        [NotMapped]
        public bool IsCurrentlyInBuilding => TimeOut == null;

        
        /// Helper property to get display-friendly contact number
        
        [NotMapped]
        public string DisplayContactNumber => ContactNumber ?? "Not provided";

       
        /// Helper property to get total room visits count
       
        [NotMapped]
        public int RoomVisitCount => RoomVisits?.Count ?? 0;
    }
}