using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLogSystem.Models
{
    
    /// User Entity - Represents the "users" table in database
    /// Stores user account information for login
    
    [Table("users")] // Name of table in database
    public class User
    {
        
      
       
        [Key]
        [Column("id")]
        public int Id { get; set; }

        
      
       
        [Required]
        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

       
       
        
        [Required]
        [Column("password_hash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        
        
        [Required]
        [Column("role")]
        [MaxLength(20)]
        public string Role { get; set; } = "Staff"; // Default role

        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}