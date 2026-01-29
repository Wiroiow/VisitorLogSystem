using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Repositories
{
    public class PreRegisteredVisitorRepository : IPreRegisteredVisitorRepository
    {
        private readonly ApplicationDbContext _context;

        public PreRegisteredVisitorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PreRegisteredVisitor> GetAll()
        {
            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Include(p => p.CheckedInByUser)
                .Include(p => p.RoomVisit)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public IEnumerable<PreRegisteredVisitor> GetPendingVisitors()
        {
            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Where(p => !p.IsCheckedIn)
                .OrderBy(p => p.ExpectedVisitDate)
                .ToList();
        }

        public IEnumerable<PreRegisteredVisitor> GetPendingVisitorsByDate(DateTime date)
        {
            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Where(p => !p.IsCheckedIn && p.ExpectedVisitDate.Date == date.Date)
                .OrderBy(p => p.ExpectedVisitDate)
                .ToList();
        }

        public IEnumerable<PreRegisteredVisitor> GetByHostUserId(int hostUserId)
        {
            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Include(p => p.CheckedInByUser)
                .Where(p => p.HostUserId == hostUserId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public PreRegisteredVisitor? GetById(int id)
        {
            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Include(p => p.CheckedInByUser)
                .Include(p => p.RoomVisit)
                .FirstOrDefault(p => p.Id == id);
        }

        public PreRegisteredVisitor Add(PreRegisteredVisitor preRegisteredVisitor)
        {
            _context.PreRegisteredVisitors.Add(preRegisteredVisitor);
            _context.SaveChanges();
            return preRegisteredVisitor;
        }

        public PreRegisteredVisitor Update(PreRegisteredVisitor preRegisteredVisitor)
        {
            _context.PreRegisteredVisitors.Update(preRegisteredVisitor);
            _context.SaveChanges();
            return preRegisteredVisitor;
        }

        public void Delete(int id)
        {
            var preRegisteredVisitor = _context.PreRegisteredVisitors.Find(id);
            if (preRegisteredVisitor != null)
            {
                _context.PreRegisteredVisitors.Remove(preRegisteredVisitor);
                _context.SaveChanges();
            }
        }

        public IEnumerable<PreRegisteredVisitor> SearchPending(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetPendingVisitors();
            }

            return _context.PreRegisteredVisitors
                .Include(p => p.HostUser)
                .Where(p => !p.IsCheckedIn &&
                           (p.FullName.Contains(searchTerm) ||
                            p.Purpose.Contains(searchTerm)))
                .OrderBy(p => p.ExpectedVisitDate)
                .ToList();
        }
    }
}