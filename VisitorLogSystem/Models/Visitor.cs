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

        
        [Column("email")]
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Column("time_in")]
        public DateTime TimeIn { get; set; }

        [Column("time_out")]
        public DateTime? TimeOut { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

     
        /// Entity Framework uses this to establish the one-to-many relationship
       
        public virtual ICollection<RoomVisit> RoomVisits { get; set; } = new List<RoomVisit>();

        
       
        [NotMapped]
        public bool IsCurrentlyInBuilding => TimeOut == null;

        
        
        [NotMapped]
        public string DisplayContactNumber => ContactNumber ?? "Not provided";

        
        
        [NotMapped]
        public string DisplayEmail => Email ?? "Not provided";

        
        [NotMapped]
        public int RoomVisitCount => RoomVisits?.Count ?? 0;
    }
}