using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly IVisitorRepository _repository;

        public VisitorService(IVisitorRepository repository)
        {
            _repository = repository;
        }

        #region CRUD Operations

        public async Task<List<VisitorDto>> GetAllVisitorsAsync()
        {
            var visitors = await _repository.GetAllAsync();
            return visitors.Select(MapToDto).ToList();
        }

        public async Task<VisitorDto?> GetVisitorByIdAsync(int id)
        {
            var visitor = await _repository.GetByIdAsync(id);

            if (visitor == null)
                return null;

            return MapToDto(visitor);
        }

        public async Task<VisitorDto> CreateVisitorAsync(VisitorDto visitorDto)
        {
            var visitor = MapToEntity(visitorDto);

            if (visitor.TimeIn == default(DateTime))
                visitor.TimeIn = DateTime.Now;

            var createdVisitor = await _repository.AddAsync(visitor);

            return MapToDto(createdVisitor);
        }

        public async Task<VisitorDto?> UpdateVisitorAsync(VisitorDto visitorDto)
        {
            var existingVisitor = await _repository.GetByIdAsync(visitorDto.Id);
            if (existingVisitor == null)
                return null;

            existingVisitor.FullName = visitorDto.FullName;
            existingVisitor.Purpose = visitorDto.Purpose;
            existingVisitor.ContactNumber = visitorDto.ContactNumber;
            existingVisitor.Email = visitorDto.Email; 
            existingVisitor.TimeIn = visitorDto.TimeIn;
            existingVisitor.TimeOut = visitorDto.TimeOut;

            var updatedVisitor = await _repository.UpdateAsync(existingVisitor);

            if (updatedVisitor == null)
                return null;

            return MapToDto(updatedVisitor);
        }

        public async Task<bool> DeleteVisitorAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        #endregion

        #region Business Operations

        public async Task<bool> CheckOutVisitorAsync(int id)
        {
            var visitor = await _repository.GetByIdAsync(id);

            if (visitor == null)
                return false;

            if (visitor.TimeOut.HasValue)
                return false;

            return await _repository.UpdateTimeOutAsync(id, DateTime.Now);
        }

        #endregion

        #region Dashboard Statistics

        public async Task<int> GetTodayVisitorCountAsync()
        {
            var visitors = await _repository.GetVisitorsTodayAsync();
            return visitors.Count;
        }

        public async Task<int> GetMonthlyVisitorCountAsync()
        {
            var visitors = await _repository.GetVisitorsThisMonthAsync();
            return visitors.Count;
        }

        public async Task<int> GetCurrentlyInsideCountAsync()
        {
            return await _repository.GetCurrentlyInsideCountAsync();
        }

        public async Task<List<VisitorDto>> GetRecentVisitorsAsync(int count)
        {
            var visitors = await _repository.GetRecentVisitorsAsync(count);
            return visitors.Select(MapToDto).ToList();
        }

        #endregion

        #region ✅ NEW: Duplicate Detection

        
        /// Find existing visitor by email (case-insensitive)
        /// Returns the most recent visitor record with this email
        
        public async Task<VisitorDto?> FindVisitorByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var visitors = await _repository.GetAllAsync();
            var visitor = visitors
                .Where(v => !string.IsNullOrWhiteSpace(v.Email) &&
                           v.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefault();

            return visitor != null ? MapToDto(visitor) : null;
        }

       
        /// Check if email already exists in the system
        
        public async Task<bool> EmailExistsAsync(string email, int? excludeVisitorId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var visitors = await _repository.GetAllAsync();
            return visitors.Any(v =>
                !string.IsNullOrWhiteSpace(v.Email) &&
                v.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                v.Id != excludeVisitorId);
        }

        
        public async Task<VisitorDto> FindOrCreateVisitorAsync(VisitorDto visitorDto)
        {
           
            if (!string.IsNullOrWhiteSpace(visitorDto.Email))
            {
                var existingVisitor = await FindVisitorByEmailAsync(visitorDto.Email);

                if (existingVisitor != null)
                {
                    
                    existingVisitor.FullName = visitorDto.FullName;
                    existingVisitor.ContactNumber = visitorDto.ContactNumber;
                    existingVisitor.Purpose = visitorDto.Purpose;

                    var updated = await UpdateVisitorAsync(existingVisitor);
                    return updated ?? existingVisitor;
                }
            }

            
            return await CreateVisitorAsync(visitorDto);
        }

        #endregion

        #region Helper Methods - Mapping

        private VisitorDto MapToDto(Visitor visitor)
        {
            return new VisitorDto
            {
                Id = visitor.Id,
                FullName = visitor.FullName,
                Purpose = visitor.Purpose,
                ContactNumber = visitor.ContactNumber,
                Email = visitor.Email, 
                TimeIn = visitor.TimeIn,
                TimeOut = visitor.TimeOut,
                CreatedAt = visitor.CreatedAt,
                UpdatedAt = visitor.UpdatedAt
            };
        }

        private Visitor MapToEntity(VisitorDto dto)
        {
            return new Visitor
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Purpose = dto.Purpose,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email, 
                TimeIn = dto.TimeIn,
                TimeOut = dto.TimeOut
            };
        }

        #endregion
    }
}