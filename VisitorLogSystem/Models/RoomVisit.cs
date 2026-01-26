using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLogSystem.Models
{
    
    
    
    /// PURPOSE: Tracks every time a visitor enters a room
   
    
   
    [Table("room_visits")]
    public class RoomVisit
    {
        
        [Key]
        [Column("id")]
        public int Id { get; set; }

       
        [Required]
        [Column("visitor_id")]
        public int VisitorId { get; set; }

        
        [ForeignKey("VisitorId")]
        public virtual Visitor? Visitor { get; set; }

       
        [Required(ErrorMessage = "Room name/number is required")]
        [Column("room_name")]
        [MaxLength(100)]
        public string RoomName { get; set; } = string.Empty;

      
        [Required]
        [Column("entered_at")]
        public DateTime EnteredAt { get; set; }

     
        [Column("purpose")]
        [MaxLength(500)]
        public string? Purpose { get; set; }

        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        [NotMapped]
        public string VisitorName => Visitor?.FullName ?? "Unknown Visitor";


        [NotMapped]
        public string VisitorContact => Visitor?.ContactNumber ?? "N/A";
    }
}