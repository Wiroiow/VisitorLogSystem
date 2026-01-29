using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    public interface IVisitorService
    {
        // CRUD Operations
        Task<List<VisitorDto>> GetAllVisitorsAsync();
        Task<VisitorDto?> GetVisitorByIdAsync(int id);
        Task<VisitorDto> CreateVisitorAsync(VisitorDto visitorDto);
        Task<VisitorDto?> UpdateVisitorAsync(VisitorDto visitorDto);
        Task<bool> DeleteVisitorAsync(int id);

        // Business Operations
        Task<bool> CheckOutVisitorAsync(int id);

        // Dashboard Statistics
        Task<int> GetTodayVisitorCountAsync();
        Task<int> GetMonthlyVisitorCountAsync();
        Task<int> GetCurrentlyInsideCountAsync();
        Task<List<VisitorDto>> GetRecentVisitorsAsync(int count);

        //Duplicate Detection
        Task<VisitorDto?> FindVisitorByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email, int? excludeVisitorId = null);
        Task<VisitorDto> FindOrCreateVisitorAsync(VisitorDto visitorDto);
    }
}