using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Interfaces
{
   
    /// PURPOSE: Defines contract for room visit data access operations
   
   
    public interface IRoomVisitRepository
    {
        /// READ OPERATIONS
        
       
        /// Get all room visits for a specific visitor
       
        Task<List<RoomVisit>> GetByVisitorIdAsync(int visitorId);
        
        
        /// Get all room visits for a specific room
       
        Task<List<RoomVisit>> GetByRoomNameAsync(string roomName);
        
       
        /// Get a single room visit by ID
        
        Task<RoomVisit?> GetByIdAsync(int id);
        
        
        /// Get all room visits within a date range
        
        Task<List<RoomVisit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
       
        /// Get the most recent room visit for a visitor
       
        Task<RoomVisit?> GetLatestByVisitorIdAsync(int visitorId);

        // Get all room visits with search and sort
        /// Get all room visits with optional search and sort
        
        Task<List<RoomVisit>> GetAllAsync(string? search = null, string? sort = null);

        // WRITE OPERATIONS
        
        
        /// Record a new room visit
        
        Task<RoomVisit> AddAsync(RoomVisit roomVisit);
        
        
        /// Update an existing room visit
      
        Task<RoomVisit?> UpdateAsync(RoomVisit roomVisit);
        
       
        /// Delete a room visit record
       
        Task<bool> DeleteAsync(int id);
    }
}