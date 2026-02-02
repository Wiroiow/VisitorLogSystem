using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.ViewModels
{
   
    /// ViewModel for RoomVisits/Index page with search and sort functionality
    
    public class RoomVisitIndexViewModel
    {
        // The list of room visits to display
        public List<RoomVisit> RoomVisits { get; set; } = new List<RoomVisit>();

        // Search functionality
        public string? SearchTerm { get; set; }

        // Sort functionality
        public string? SortOption { get; set; }

        // Dropdown options for sorting
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Default (Newest First)" },
            new SelectListItem { Value = "NameAsc", Text = "Visitor Name (A-Z)" },
            new SelectListItem { Value = "NameDesc", Text = "Visitor Name (Z-A)" },
            new SelectListItem { Value = "DateNewest", Text = "Date (Newest First)" },
            new SelectListItem { Value = "DateOldest", Text = "Date (Oldest First)" },
            new SelectListItem { Value = "Status", Text = "Status (Active First)" },
            new SelectListItem { Value = "Room", Text = "Room Name (A-Z)" }
        };
    }
}