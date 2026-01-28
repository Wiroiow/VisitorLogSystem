using System;
using System.Collections.Generic;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Interfaces
{
    public interface IPreRegisteredVisitorRepository
    {
        IEnumerable<PreRegisteredVisitor> GetAll();
        IEnumerable<PreRegisteredVisitor> GetPendingVisitors();
        IEnumerable<PreRegisteredVisitor> GetPendingVisitorsByDate(DateTime date);
        IEnumerable<PreRegisteredVisitor> GetByHostUserId(int hostUserId);
        PreRegisteredVisitor GetById(int id);
        PreRegisteredVisitor Add(PreRegisteredVisitor preRegisteredVisitor);
        PreRegisteredVisitor Update(PreRegisteredVisitor preRegisteredVisitor);
        void Delete(int id);
        IEnumerable<PreRegisteredVisitor> SearchPending(string searchTerm);
    }
}