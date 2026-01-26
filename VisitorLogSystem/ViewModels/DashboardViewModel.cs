using System.Collections.Generic;

namespace VisitorLogSystem.ViewModels
{
   
   
    /// Contains all statistics and data needed for the dashboard
   
    public class DashboardViewModel
    {
       
        public int TotalVisitorsToday { get; set; }

        
        public int CurrentlyInside { get; set; }

      
        public int MonthlyVisitors { get; set; }

       
        public List<VisitorViewModel> RecentVisitors { get; set; }

        public DashboardViewModel()
        {
            RecentVisitors = new List<VisitorViewModel>();
        }
    }
}