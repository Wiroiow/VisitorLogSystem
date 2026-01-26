using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Controllers
{
    public class VisitorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var visitors = await _context.Visitors
                .Include(v => v.RoomVisits)
                .OrderByDescending(v => v.TimeIn)
                .ToListAsync();

            return View(visitors);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.RoomVisits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Purpose,ContactNumber,TimeIn")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                visitor.CreatedAt = DateTime.Now;
                visitor.UpdatedAt = DateTime.Now;

                // Set TimeIn to now if not specified
                if (visitor.TimeIn == default)
                {
                    visitor.TimeIn = DateTime.Now;
                }

                _context.Add(visitor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{visitor.FullName} has been registered successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(visitor);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            return View(visitor);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Purpose,ContactNumber,TimeIn,TimeOut,CreatedAt")] Visitor visitor)
        {
            if (id != visitor.Id)
            {
                return NotFound();
            }

            // Remove TimeOut from ModelState if it's empty (to allow null values)
            if (string.IsNullOrWhiteSpace(Request.Form["TimeOut"]))
            {
                ModelState.Remove("TimeOut");
                visitor.TimeOut = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    visitor.UpdatedAt = DateTime.Now;
                    _context.Update(visitor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"{visitor.FullName}'s information has been updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorExists(visitor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If we got here, something failed, redisplay form
            return View(visitor);
        }

        private bool VisitorExists(int id)
        {
            throw new NotImplementedException();
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.RoomVisits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitor = await _context.Visitors
                .Include(v => v.RoomVisits)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor != null)
            {
                // Delete associated room visits first
                if (visitor.RoomVisits != null && visitor.RoomVisits.Any())
                {
                    _context.RoomVisits.RemoveRange(visitor.RoomVisits);
                }

                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{visitor.FullName}'s record has been deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> SignOut(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.RoomVisits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            if (visitor.TimeOut.HasValue)
            {
                TempData["ErrorMessage"] = $"{visitor.FullName} has already signed out.";
                return RedirectToAction(nameof(Index));
            }

            return View(visitor);
        }

        
        [HttpPost, ActionName("SignOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOutConfirmed(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);

            if (visitor != null && !visitor.TimeOut.HasValue)
            {
                visitor.TimeOut = DateTime.Now;
                visitor.UpdatedAt = DateTime.Now;

                _context.Update(visitor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{visitor.FullName} has been signed out successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
