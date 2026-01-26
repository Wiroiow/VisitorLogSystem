using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Interfaces
{
    public interface IVisitorRepository
    {
        // READ operations
        Task<List<Visitor>> GetAllAsync();
        Task<Visitor?> GetByIdAsync(int id); 
        Task<List<Visitor>> GetVisitorsTodayAsync();
        Task<List<Visitor>> GetVisitorsThisMonthAsync();
        Task<List<Visitor>> GetRecentVisitorsAsync(int count);
        Task<int> GetCurrentlyInsideCountAsync();

        // WRITE operations
        Task<Visitor> AddAsync(Visitor visitor);
        Task<Visitor?> UpdateAsync(Visitor visitor); 
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateTimeOutAsync(int id, DateTime timeOut);
    }
}