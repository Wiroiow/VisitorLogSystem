using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
   
    /// RESPONSIBILITY: Business logic for room tracking
  
    public class RoomVisitService : IRoomVisitService
    {
        private readonly IRoomVisitRepository _roomVisitRepository;
        private readonly IVisitorRepository _visitorRepository;

       
     
      
        public RoomVisitService(
            IRoomVisitRepository roomVisitRepository,
            IVisitorRepository visitorRepository)
        {
            _roomVisitRepository = roomVisitRepository;
            _visitorRepository = visitorRepository;
        }

       
        /// Record a visitor entering a room
      
      
      
        public async Task<RoomVisitDto> RecordRoomEntryAsync(
            int visitorId,
            string roomName,
            string? purpose = null)
        {
           
            // STEP 1: Validate visitor exists and is checked-in
           

            var visitor = await _visitorRepository.GetByIdAsync(visitorId);

            // Business Rule: Visitor must exist
            if (visitor == null)
            {
                throw new InvalidOperationException($"Visitor with ID {visitorId} not found.");
            }

            // Business Rule: Visitor must be checked-in (still in building)
            if (visitor.TimeOut.HasValue)
            {
                throw new InvalidOperationException(
                    $"Cannot record room entry. {visitor.FullName} has already checked out at {visitor.TimeOut}."
                );
            }

            
            // STEP 2: Validate room name
         

            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new ArgumentException("Room name cannot be empty.", nameof(roomName));
            }

           
            // STEP 3: Create room visit entity
         

            var roomVisit = new RoomVisit
            {
                VisitorId = visitorId,
                RoomName = roomName.Trim(), // Remove extra whitespace
                EnteredAt = DateTime.Now,   
                Purpose = purpose?.Trim()   // Optional purpose
            };

          
            // STEP 4: Save to database
           

            var createdVisit = await _roomVisitRepository.AddAsync(roomVisit);

          
            // STEP 5: Convert Entity to DTO and return
            

            return MapToDto(createdVisit, visitor);
        }

       
        /// Get complete room history for a visitor
        
        public async Task<List<RoomVisitDto>> GetVisitorRoomHistoryAsync(int visitorId)
        {
            // Get all room visits for this visitor
            var roomVisits = await _roomVisitRepository.GetByVisitorIdAsync(visitorId);

            // Convert each Entity to DTO
            // The Visitor property is already loaded (by Include in repository)
            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        
        /// Get all visitors who entered a specific room
        
        public async Task<List<RoomVisitDto>> GetRoomVisitorsAsync(string roomName)
        {
            // Business Rule: Room name required
            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new ArgumentException("Room name cannot be empty.", nameof(roomName));
            }

            var roomVisits = await _roomVisitRepository.GetByRoomNameAsync(roomName);

            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        
        /// Get visitor's current room location
        
        public async Task<RoomVisitDto?> GetCurrentRoomAsync(int visitorId)
        {
            var latestVisit = await _roomVisitRepository.GetLatestByVisitorIdAsync(visitorId);

            
            if (latestVisit == null)
            {
                return null;
            }

            return MapToDto(latestVisit, latestVisit.Visitor);
        }

        
        /// Get room visits within date range
       
        public async Task<List<RoomVisitDto>> GetRoomVisitsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            // Business Rule: Start date must be before end date
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            var roomVisits = await _roomVisitRepository.GetByDateRangeAsync(startDate, endDate);

            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        // HELPER METHODS
        

        
        /// Convert RoomVisit entity to RoomVisitDto
       
        private RoomVisitDto MapToDto(RoomVisit roomVisit, Visitor? visitor)
        {
            return new RoomVisitDto
            {
                Id = roomVisit.Id,
                VisitorId = roomVisit.VisitorId,
                VisitorName = visitor?.FullName, 
                RoomName = roomVisit.RoomName,
                EnteredAt = roomVisit.EnteredAt,
                Purpose = roomVisit.Purpose,
                CreatedAt = roomVisit.CreatedAt
            };
        }
    }
}