using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
    public class PreRegisteredVisitorService : IPreRegisteredVisitorService
    {
        private readonly IPreRegisteredVisitorRepository _preRegRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly IRoomVisitRepository _roomVisitRepository;

        public PreRegisteredVisitorService(
            IPreRegisteredVisitorRepository preRegRepository,
            IVisitorRepository visitorRepository,
            IRoomVisitRepository roomVisitRepository)
        {
            _preRegRepository = preRegRepository;
            _visitorRepository = visitorRepository;
            _roomVisitRepository = roomVisitRepository;
        }

        public IEnumerable<PreRegisteredVisitorDto> GetAllPreRegisteredVisitors()
        {
            var preRegistrations = _preRegRepository.GetAll();
            return preRegistrations.Select(MapToDto);
        }

        public IEnumerable<PreRegisteredVisitorDto> GetPendingVisitors()
        {
            var pending = _preRegRepository.GetPendingVisitors();
            return pending.Select(MapToDto);
        }

        public IEnumerable<PreRegisteredVisitorDto> GetPendingVisitorsByDate(DateTime date)
        {
            var pending = _preRegRepository.GetPendingVisitorsByDate(date);
            return pending.Select(MapToDto);
        }

        public IEnumerable<PreRegisteredVisitorDto> GetByHostUserId(int hostUserId)
        {
            var preRegistrations = _preRegRepository.GetByHostUserId(hostUserId);
            return preRegistrations.Select(MapToDto);
        }

        public PreRegisteredVisitorDto? GetById(int id)
        {
            var preRegistration = _preRegRepository.GetById(id);
            return preRegistration != null ? MapToDto(preRegistration) : null;
        }

        public PreRegisteredVisitorDto CreatePreRegistration(PreRegisteredVisitorDto dto)
        {
            var preRegistration = new PreRegisteredVisitor
            {
                FullName = dto.FullName,
                Purpose = dto.Purpose?? string.Empty,
                ExpectedVisitDate = dto.ExpectedVisitDate,
                HostUserId = dto.HostUserId,
                IsCheckedIn = false,
                CreatedAt = DateTime.Now
            };

            var created = _preRegRepository.Add(preRegistration);
            return MapToDto(created);
        }

        public PreRegisteredVisitorDto UpdatePreRegistration(PreRegisteredVisitorDto dto)
        {
            var existing = _preRegRepository.GetById(dto.Id);
            if (existing == null)
            {
                throw new Exception("Pre-registration not found");
            }

            if (existing.IsCheckedIn)
            {
                throw new Exception("Cannot update a pre-registration that has been checked in");
            }

            existing.FullName = dto.FullName;
            existing.Purpose = dto.Purpose ?? string.Empty;
            existing.ExpectedVisitDate = dto.ExpectedVisitDate;
            existing.HostUserId = dto.HostUserId;

            var updated = _preRegRepository.Update(existing);
            return MapToDto(updated);
        }

        public void DeletePreRegistration(int id)
        {
            var existing = _preRegRepository.GetById(id);
            if (existing == null)
            {
                throw new Exception("Pre-registration not found");
            }

            if (existing.IsCheckedIn)
            {
                throw new Exception("Cannot delete a pre-registration that has been checked in");
            }

            _preRegRepository.Delete(id);
        }

        public IEnumerable<PreRegisteredVisitorDto> SearchPending(string searchTerm)
        {
            var results = _preRegRepository.SearchPending(searchTerm);
            return results.Select(MapToDto);
        }

        public async Task<RoomVisitDto> CheckInPreRegisteredVisitorAsync(int preRegistrationId, int checkedInByUserId)
        {
            var preReg = _preRegRepository.GetById(preRegistrationId);
            if (preReg == null)
            {
                throw new Exception("Pre-registration not found");
            }

            if (preReg.IsCheckedIn)
            {
                throw new Exception("This visitor has already been checked in");
            }

            // Find or create visitor
            var visitors = await _visitorRepository.GetAllAsync();
            var visitor = visitors.FirstOrDefault(v =>
                v.FullName.Equals(preReg.FullName, StringComparison.OrdinalIgnoreCase));

            if (visitor == null)
            {
                visitor = new Visitor
                {
                    FullName = preReg.FullName,
                    CreatedAt = DateTime.Now
                };
                visitor = await _visitorRepository.AddAsync(visitor);
            }

            // Create RoomVisit - Note: Your RoomVisit model requires RoomName
            var roomVisit = new RoomVisit
            {
                VisitorId = visitor.Id,
                RoomName = "Main Office", // Default room - you may want to add this to PreRegisteredVisitor
                Purpose = preReg.Purpose,
                EnteredAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };
            roomVisit = await _roomVisitRepository.AddAsync(roomVisit);

            // Update PreRegistration
            preReg.IsCheckedIn = true;
            preReg.CheckedInByUserId = checkedInByUserId;
            preReg.CheckedInAt = DateTime.Now;
            preReg.RoomVisitId = roomVisit.Id;
            _preRegRepository.Update(preReg);

            // Return the created RoomVisit as DTO
            var roomVisitWithRelations = await _roomVisitRepository.GetByIdAsync(roomVisit.Id);

            if (roomVisitWithRelations == null)
            {
                throw new Exception("Failed to retrieve room visit after creation");
            }
            // Your RoomVisit model doesn't have CreatedByUser, so we'll use available data
            return new RoomVisitDto
            {
                Id = roomVisitWithRelations.Id,
                VisitorId = roomVisitWithRelations.VisitorId,
                FullName = roomVisitWithRelations.Visitor?.FullName ?? "",
                Purpose = roomVisitWithRelations.Purpose ?? "",
                TimeIn = roomVisitWithRelations.EnteredAt,
                TimeOut = null, // Your model doesn't track checkout
                UserId = checkedInByUserId, // Pass through the user who checked them in
                Username = "" // Your RoomVisit doesn't have User navigation property
            };
        }

        private PreRegisteredVisitorDto MapToDto(PreRegisteredVisitor model)
        {
            return new PreRegisteredVisitorDto
            {
                Id = model.Id,
                FullName = model.FullName,
                Purpose = model.Purpose,
                ExpectedVisitDate = model.ExpectedVisitDate,
                HostUserId = model.HostUserId,
                HostUserName = model.HostUser?.Username,
                IsCheckedIn = model.IsCheckedIn,
                CreatedAt = model.CreatedAt,
                CheckedInByUserId = model.CheckedInByUserId,
                CheckedInByUserName = model.CheckedInByUser?.Username,
                CheckedInAt = model.CheckedInAt,
                RoomVisitId = model.RoomVisitId
            };
        }
    }
}