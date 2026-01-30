using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Controllers
{
    public class RoomVisitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomVisitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RoomVisits
        public async Task<IActionResult> Index()
        {
            var roomVisits = await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .OrderByDescending(rv => rv.EnteredAt)
                .ToListAsync();

            return View(roomVisits);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomVisit = await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (roomVisit == null)
            {
                return NotFound();
            }

            return View(roomVisit);
        }

        public async Task<IActionResult> Create()
        {
            // Load only visitors currently in the building 
            var activeVisitors = await _context.Visitors
                .Where(v => v.TimeOut == null)
                .OrderBy(v => v.FullName)
                .Select(v => new
                {
                    v.Id,
                    DisplayText = $"{v.FullName} - Entered at {v.TimeIn:HH:mm}"
                })
                .ToListAsync();

            ViewData["VisitorId"] = new SelectList(activeVisitors, "Id", "DisplayText");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitorId,RoomName,EnteredAt,Purpose")] RoomVisit roomVisit)
        {
            if (ModelState.IsValid)
            {
                // Verify visitor exists and is still in building
                var visitor = await _context.Visitors.FindAsync(roomVisit.VisitorId);

                if (visitor == null)
                {
                    ModelState.AddModelError("VisitorId", "Selected visitor does not exist.");
                }
                else if (visitor.TimeOut != null)
                {
                    ModelState.AddModelError("VisitorId", "Selected visitor has already left the building.");
                }
                else
                {
                    roomVisit.CreatedAt = DateTime.Now;

                    // Set EnteredAt to now if not specified
                    if (roomVisit.EnteredAt == default)
                    {
                        roomVisit.EnteredAt = DateTime.Now;
                    }

                    _context.Add(roomVisit);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Room visit to {roomVisit.RoomName} recorded successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload dropdown if validation fails
            var activeVisitors = await _context.Visitors
                .Where(v => v.TimeOut == null)
                .OrderBy(v => v.FullName)
                .Select(v => new
                {
                    v.Id,
                    DisplayText = $"{v.FullName} - Entered at {v.TimeIn:HH:mm}"
                })
                .ToListAsync();

            ViewData["VisitorId"] = new SelectList(activeVisitors, "Id", "DisplayText", roomVisit.VisitorId);

            return View(roomVisit);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomVisit = await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .FirstOrDefaultAsync(rv => rv.Id == id);

            if (roomVisit == null)
            {
                return NotFound();
            }

            // Check if visitor has signed out
            if (roomVisit.Visitor?.TimeOut.HasValue == true)
            {
                TempData["ErrorMessage"] = "Cannot edit room visit - visitor has already signed out.";
                return RedirectToAction(nameof(Index));
            }

            // Load all visitors for dropdown
            var visitors = await _context.Visitors
                .OrderBy(v => v.FullName)
                .Select(v => new
                {
                    v.Id,
                    DisplayText = v.FullName
                })
                .ToListAsync();

            ViewData["VisitorId"] = new SelectList(visitors, "Id", "DisplayText", roomVisit.VisitorId);

            return View(roomVisit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VisitorId,RoomName,EnteredAt,Purpose,CreatedAt")] RoomVisit roomVisit)
        {
            if (id != roomVisit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verify visitor exists and hasn't signed out
                    var visitor = await _context.Visitors.FindAsync(roomVisit.VisitorId);

                    if (visitor == null)
                    {
                        ModelState.AddModelError("VisitorId", "Selected visitor does not exist.");
                    }
                    else if (visitor.TimeOut.HasValue)
                    {
                        ModelState.AddModelError("VisitorId", "Cannot update - visitor has already signed out.");
                    }
                    else
                    {
                        _context.Update(roomVisit);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Room visit updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomVisitExists(roomVisit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Reload dropdown if validation fails
            var visitors = await _context.Visitors
                .OrderBy(v => v.FullName)
                .Select(v => new
                {
                    v.Id,
                    DisplayText = v.FullName
                })
                .ToListAsync();

            ViewData["VisitorId"] = new SelectList(visitors, "Id", "DisplayText", roomVisit.VisitorId);

            return View(roomVisit);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomVisit = await _context.RoomVisits
                .Include(rv => rv.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (roomVisit == null)
            {
                return NotFound();
            }

            return View(roomVisit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomVisit = await _context.RoomVisits.FindAsync(id);

            if (roomVisit != null)
            {
                _context.RoomVisits.Remove(roomVisit);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room visit deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> ByVisitor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.RoomVisits)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            ViewData["VisitorName"] = visitor.FullName;

            //FIX: Handle null collection and ensure proper typing
            var roomVisits = visitor.RoomVisits?.OrderByDescending(rv => rv.EnteredAt).ToList()
                             ?? new List<RoomVisit>();

            return View(roomVisits);
        }

        private bool RoomVisitExists(int id)
        {
            return _context.RoomVisits.Any(e => e.Id == id);
        }
    }
}