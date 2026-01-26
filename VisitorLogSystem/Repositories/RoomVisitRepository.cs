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