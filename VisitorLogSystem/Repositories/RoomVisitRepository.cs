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

    /// RESPONSIBILITY: Handle ALL database operations for room_visits table
    /// - ONLY database access (no business logic)

    public class RoomVisitRepository : IRoomVisitRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomVisitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ OPERATIONS


        /// Get all room visits for a specific visitor

        public async Task<List<RoomVisit>> GetByVisitorIdAsync(int visitorId)
        {
            return await _context.RoomVisits
                .Include(rv => rv.Visitor) // Load visitor info too
                .Where(rv => rv.VisitorId == visitorId)
                .OrderBy(rv => rv.EnteredAt) // Chronological order
                .ToListAsync();
        }


        /// Get all room visits for a specific room

        public async Task<List<RoomVisit>> GetByRoomNameAsync(string roomName)
        {
            return await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .Where(rv => rv.RoomName == roomName)
                .OrderByDescending(rv => rv.EnteredAt)
                .ToListAsync();
        }


        /// Get single room visit by ID

        public async Task<RoomVisit?> GetByIdAsync(int id)
        {
            return await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .FirstOrDefaultAsync(rv => rv.Id == id);
        }


        /// Get room visits within date range

        public async Task<List<RoomVisit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .Where(rv => rv.EnteredAt >= startDate && rv.EnteredAt < endDate)
                .OrderBy(rv => rv.EnteredAt)
                .ToListAsync();
        }


        /// Get most recent room visit for a visitor

        public async Task<RoomVisit?> GetLatestByVisitorIdAsync(int visitorId)
        {
            return await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .Where(rv => rv.VisitorId == visitorId)
                .OrderByDescending(rv => rv.EnteredAt)
                .FirstOrDefaultAsync(); // Get the first (most recent)
        }

        //Get all room visits with search and sort
        public async Task<List<RoomVisit>> GetAllAsync(string? search = null, string? sort = null)
        {
            // Start with base query
            IQueryable<RoomVisit> query = _context.RoomVisits
                .Include(rv => rv.Visitor);

            // Apply search filter (partial match across multiple fields)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(rv =>
                    rv.RoomName.ToLower().Contains(search) ||
                    (rv.Visitor != null && rv.Visitor.FullName.ToLower().Contains(search)) ||
                    (rv.Purpose != null && rv.Purpose.ToLower().Contains(search)) ||
                    (rv.Visitor != null && rv.Visitor.ContactNumber != null && rv.Visitor.ContactNumber.Contains(search))
                );
            }

            // Apply sorting
            query = sort switch
            {
                "NameAsc" => query.OrderBy(rv => rv.Visitor != null ? rv.Visitor.FullName : ""),
                "NameDesc" => query.OrderByDescending(rv => rv.Visitor != null ? rv.Visitor.FullName : ""),
                "DateNewest" => query.OrderByDescending(rv => rv.EnteredAt),
                "DateOldest" => query.OrderBy(rv => rv.EnteredAt),
                "Status" => query.OrderBy(rv => rv.Visitor != null && rv.Visitor.TimeOut.HasValue)
                                 .ThenByDescending(rv => rv.EnteredAt),
                "Room" => query.OrderBy(rv => rv.RoomName).ThenByDescending(rv => rv.EnteredAt),
                _ => query.OrderByDescending(rv => rv.EnteredAt) // Default sort
            };

            return await query.ToListAsync();
        }

        /// Get paginated room visits with search and sort
        /// Returns tuple of (room visits, total count)
        public async Task<(List<RoomVisit> RoomVisits, int TotalCount)> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            string? sort = null)
        {
            // Start with base query
            IQueryable<RoomVisit> query = _context.RoomVisits
                .Include(rv => rv.Visitor);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(rv =>
                    rv.RoomName.ToLower().Contains(search) ||
                    (rv.Visitor != null && rv.Visitor.FullName.ToLower().Contains(search)) ||
                    (rv.Purpose != null && rv.Purpose.ToLower().Contains(search)) ||
                    (rv.Visitor != null && rv.Visitor.ContactNumber != null && rv.Visitor.ContactNumber.Contains(search))
                );
            }

            // Get total count BEFORE pagination
            int totalCount = await query.CountAsync();

            // Apply sorting
            query = sort switch
            {
                "NameAsc" => query.OrderBy(rv => rv.Visitor != null ? rv.Visitor.FullName : ""),
                "NameDesc" => query.OrderByDescending(rv => rv.Visitor != null ? rv.Visitor.FullName : ""),
                "DateNewest" => query.OrderByDescending(rv => rv.EnteredAt),
                "DateOldest" => query.OrderBy(rv => rv.EnteredAt),
                "Status" => query.OrderBy(rv => rv.Visitor != null && rv.Visitor.TimeOut.HasValue)
                                 .ThenByDescending(rv => rv.EnteredAt),
                "Room" => query.OrderBy(rv => rv.RoomName).ThenByDescending(rv => rv.EnteredAt),
                _ => query.OrderByDescending(rv => rv.EnteredAt)
            };

            // Apply pagination using Skip and Take
            var roomVisits = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (roomVisits, totalCount);
        }

        // WRITE OPERATIONS


        /// Add new room visit to database

        public async Task<RoomVisit> AddAsync(RoomVisit roomVisit)
        {
            roomVisit.CreatedAt = DateTime.Now;
            _context.RoomVisits.Add(roomVisit);
            await _context.SaveChangesAsync();
            return roomVisit;
        }


        /// Update existing room visit

        public async Task<RoomVisit?> UpdateAsync(RoomVisit roomVisit)
        {
            try
            {
                _context.Entry(roomVisit).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return roomVisit;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }


        /// Delete room visit by ID

        public async Task<bool> DeleteAsync(int id)
        {
            var roomVisit = await GetByIdAsync(id);

            if (roomVisit == null)
            {
                return false;
            }

            _context.RoomVisits.Remove(roomVisit);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}