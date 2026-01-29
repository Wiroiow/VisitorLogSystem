using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    public interface IPreRegisteredVisitorService
    {
        IEnumerable<PreRegisteredVisitorDto> GetAllPreRegisteredVisitors();
        IEnumerable<PreRegisteredVisitorDto> GetPendingVisitors();
        IEnumerable<PreRegisteredVisitorDto> GetPendingVisitorsByDate(DateTime date);
        IEnumerable<PreRegisteredVisitorDto> GetByHostUserId(int hostUserId);
        PreRegisteredVisitorDto? GetById(int id);
        PreRegisteredVisitorDto CreatePreRegistration(PreRegisteredVisitorDto dto);
        PreRegisteredVisitorDto UpdatePreRegistration(PreRegisteredVisitorDto dto);
        void DeletePreRegistration(int id);
        IEnumerable<PreRegisteredVisitorDto> SearchPending(string searchTerm);
        Task<RoomVisitDto> CheckInPreRegisteredVisitorAsync(int preRegistrationId, int checkedInByUserId);
    }
}