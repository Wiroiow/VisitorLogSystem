using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
 
    /// PURPOSE: Defines contract for room visit business logic
   
   
    public interface IRoomVisitService
    {
      
        /// Record a visitor entering a room
       
        Task<RoomVisitDto> RecordRoomEntryAsync(int visitorId, string roomName, string? purpose = null);

       
        /// Get all rooms visited by a visitor
      
        Task<List<RoomVisitDto>> GetVisitorRoomHistoryAsync(int visitorId);

        
        /// Get all visitors who entered a specific room
        
        Task<List<RoomVisitDto>> GetRoomVisitorsAsync(string roomName);

       
        /// Get current room location for a visitor
        
        Task<RoomVisitDto?> GetCurrentRoomAsync(int visitorId);

        
        /// Get room visits within a date range
        
        Task<List<RoomVisitDto>> GetRoomVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}