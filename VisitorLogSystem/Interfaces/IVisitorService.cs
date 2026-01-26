using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    public interface IVisitorService
    {
        // CRUD operations
        Task<List<VisitorDto>> GetAllVisitorsAsync();
        Task<VisitorDto?> GetVisitorByIdAsync(int id); 
        Task<VisitorDto> CreateVisitorAsync(VisitorDto visitorDto);
        Task<VisitorDto?> UpdateVisitorAsync(VisitorDto visitorDto);
        Task<bool> DeleteVisitorAsync(int id);

        // Business operations
        Task<bool> CheckOutVisitorAsync(int id);

        // Dashboard statistics
        Task<int> GetTodayVisitorCountAsync();
        Task<int> GetMonthlyVisitorCountAsync();
        Task<int> GetCurrentlyInsideCountAsync();
        Task<List<VisitorDto>> GetRecentVisitorsAsync(int count);
    }
}