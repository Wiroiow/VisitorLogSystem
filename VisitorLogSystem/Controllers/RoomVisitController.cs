using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    public class RoomVisitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomVisitService _roomVisitService;

        // ✅ UPDATED: Add IRoomVisitService dependency
        public RoomVisitsController(ApplicationDbContext context, IRoomVisitService roomVisitService)
        {
            _context = context;
            _roomVisitService = roomVisitService;
        }

        // ✅ UPDATED: Use ViewModel with search/sort, pagination, AND active filter
        public async Task<IActionResult> Index(string? search, string? sort, bool? activeOnly, int page = 1)
        {
            const int pageSize = 10;

            // Validate page number
            if (page < 1) page = 1;

            // Use repository query with search, sort, and pagination
            var query = _context.RoomVisits
                .Include(rv => rv.Visitor)
                .AsQueryable();

            // ✅ NEW: Apply active filter (only show room visits where visitor is still in building)
            if (activeOnly == true)
            {
                query = query.Where(rv => rv.Visitor != null && rv.Visitor.TimeOut == null);
            }

            // Apply search and sort
            query = query.ApplySearchAndSort(search, sort);

            // Get total count for pagination
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var roomVisits = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new RoomVisitIndexViewModel
            {
                RoomVisits = roomVisits,
                SearchTerm = search,
                SortOption = sort,
                ShowActiveOnly = activeOnly ?? false, 
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return View(viewModel);
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

            //Handle null collection and ensure proper typing
            var roomVisits = visitor.RoomVisits?.OrderByDescending(rv => rv.EnteredAt).ToList()
                             ?? new List<RoomVisit>();

            return View(roomVisits);
        }

        private bool RoomVisitExists(int id)
        {
            return _context.RoomVisits.Any(e => e.Id == id);
        }
    }

    //Extension method for applying search and sort
    public static class RoomVisitQueryExtensions
    {
        public static IQueryable<RoomVisit> ApplySearchAndSort(
            this IQueryable<RoomVisit> query,
            string? search,
            string? sort)
        {
            // Apply search
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

            // Apply sort
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

            return query;
        }
    }
}