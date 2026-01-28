using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
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
                RoomName = roomName.Trim(),
                EnteredAt = DateTime.Now,
                Purpose = purpose?.Trim(),
                CreatedAt = DateTime.Now
            };

            // STEP 4: Save to database
            var createdVisit = await _roomVisitRepository.AddAsync(roomVisit);

            // STEP 5: Convert Entity to DTO and return
            return MapToDto(createdVisit, visitor);
        }

        public async Task<List<RoomVisitDto>> GetVisitorRoomHistoryAsync(int visitorId)
        {
            var roomVisits = await _roomVisitRepository.GetByVisitorIdAsync(visitorId);
            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        public async Task<List<RoomVisitDto>> GetRoomVisitorsAsync(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new ArgumentException("Room name cannot be empty.", nameof(roomName));
            }

            var roomVisits = await _roomVisitRepository.GetByRoomNameAsync(roomName);
            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        public async Task<RoomVisitDto?> GetCurrentRoomAsync(int visitorId)
        {
            var latestVisit = await _roomVisitRepository.GetLatestByVisitorIdAsync(visitorId);

            if (latestVisit == null)
            {
                return null;
            }

            return MapToDto(latestVisit, latestVisit.Visitor);
        }

        public async Task<List<RoomVisitDto>> GetRoomVisitsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            var roomVisits = await _roomVisitRepository.GetByDateRangeAsync(startDate, endDate);
            return roomVisits.Select(rv => MapToDto(rv, rv.Visitor)).ToList();
        }

        // HELPER METHOD - ✅ CORRECTED to match your RoomVisitDto
        private RoomVisitDto MapToDto(RoomVisit roomVisit, Visitor? visitor)
        {
            return new RoomVisitDto
            {
                Id = roomVisit.Id,
                VisitorId = roomVisit.VisitorId,
                FullName = visitor?.FullName ?? "Unknown",  
                Purpose = roomVisit.Purpose ?? "",
                TimeIn = roomVisit.EnteredAt,               
                TimeOut = null,                            
                UserId = 0,                                
                Username = ""                              
            };
        }
    }
}