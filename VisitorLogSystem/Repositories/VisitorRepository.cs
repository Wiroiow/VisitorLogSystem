using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Repositories
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Visitor>> GetAllAsync()
        {
            return await _context.Visitors
                .OrderByDescending(v => v.TimeIn)
                .ToListAsync();
        }

        public async Task<Visitor?> GetByIdAsync(int id) 
        {
            return await _context.Visitors
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Visitor>> GetVisitorsTodayAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Visitors
                .Where(v => v.TimeIn >= today && v.TimeIn < tomorrow)
                .OrderByDescending(v => v.TimeIn)
                .ToListAsync();
        }

        public async Task<List<Visitor>> GetVisitorsThisMonthAsync()
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            return await _context.Visitors
                .Where(v => v.TimeIn >= firstDayOfMonth && v.TimeIn < firstDayOfNextMonth)
                .ToListAsync();
        }

        public async Task<List<Visitor>> GetRecentVisitorsAsync(int count)
        {
            return await _context.Visitors
                .OrderByDescending(v => v.TimeIn)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetCurrentlyInsideCountAsync()
        {
            return await _context.Visitors
                .CountAsync(v => v.TimeOut == null);
        }

        public async Task<Visitor> AddAsync(Visitor visitor)
        {
            visitor.CreatedAt = DateTime.Now;
            visitor.UpdatedAt = DateTime.Now;

            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();

            return visitor;
        }

        public async Task<Visitor?> UpdateAsync(Visitor visitor) 
        {
            visitor.UpdatedAt = DateTime.Now;
            _context.Entry(visitor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return visitor;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null; 
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var visitor = await GetByIdAsync(id);
            if (visitor == null)
                return false;

            _context.Visitors.Remove(visitor);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateTimeOutAsync(int id, DateTime timeOut)
        {
            var visitor = await GetByIdAsync(id);
            if (visitor == null)
                return false;

            visitor.TimeOut = timeOut;
            visitor.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}