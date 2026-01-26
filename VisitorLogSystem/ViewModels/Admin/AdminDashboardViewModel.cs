using System.Collections.Generic;

namespace VisitorLogSystem.ViewModels.Admin
{
    
   
    /// Shows admin-specific statistics and information
   
    public class AdminDashboardViewModel
    {
       
        /// Total number of users in the system
       
        public int TotalUsers { get; set; }

       
        public int TotalAdmins { get; set; }

        
        public int TotalStaff { get; set; }

       
        public int TotalVisitors { get; set; }

       
        public int VisitorsToday { get; set; }

        
        public int VisitorsThisMonth { get; set; }

       
        public List<UserViewModel> RecentUsers { get; set; }

        
        public List<VisitorViewModel> RecentActivity { get; set; }

        public AdminDashboardViewModel()
        {
            RecentUsers = new List<UserViewModel>();
            RecentActivity = new List<VisitorViewModel>();
        }
    }
}