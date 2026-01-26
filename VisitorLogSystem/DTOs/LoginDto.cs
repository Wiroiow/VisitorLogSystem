using System;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Models;
using static System.Net.Mime.MediaTypeNames;

namespace VisitorLogSystem.DTOs
{
    
    /// Login DTO - Transfers login data between Service and Controller
   
    public class LoginDto
    {
      
        public string Username { get; set; } = string.Empty;

        
        public string Password { get; set; } = string.Empty;

     
        public bool RememberMe { get; set; }
    }
}
