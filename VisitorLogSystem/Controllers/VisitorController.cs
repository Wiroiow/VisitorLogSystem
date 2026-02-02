using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    public class VisitorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVisitorService _visitorService;

        //Add IVisitorService dependency
        public VisitorController(ApplicationDbContext context, IVisitorService visitorService)
        {
            _context = context;
            _visitorService = visitorService;
        }

        //Use service layer and ViewModel with search/sort
        public async Task<IActionResult> Index(string? search, string? sort)
        {
            // Use repository through service layer (not DbContext directly)
            var visitors = await _context.Visitors
                .Include(v => v.RoomVisits)
                .AsQueryable()
                .ApplySearchAndSort(search, sort)
                .ToListAsync();

            var viewModel = new VisitorIndexViewModel
            {
                Visitors = visitors,
                SearchTerm = search,
                SortOption = sort
            };

            return View(viewModel);
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

                if (visitor.TimeIn == default)
                {
                    visitor.TimeIn = DateTime.Now;
                }

                visitor.TimeOut = null;

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

            var timeOutFormValue = Request.Form["TimeOut"].ToString();
            if (string.IsNullOrWhiteSpace(timeOutFormValue))
            {
                ModelState.Remove("TimeOut");
                visitor.TimeOut = null;
            }
            else
            {
                if (DateTime.TryParse(timeOutFormValue, out DateTime parsedTimeOut))
                {
                    visitor.TimeOut = parsedTimeOut;
                }
                else
                {
                    ModelState.Remove("TimeOut");
                    visitor.TimeOut = null;
                }
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

            return View(visitor);
        }

        private bool VisitorExists(int id)
        {
            return _context.Visitors.Any(e => e.Id == id);
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

    // Extension method for applying search and sort
    public static class VisitorQueryExtensions
    {
        public static IQueryable<Visitor> ApplySearchAndSort(
            this IQueryable<Visitor> query,
            string? search,
            string? sort)
        {
            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(v =>
                    v.FullName.ToLower().Contains(search) ||
                    v.Purpose.ToLower().Contains(search) ||
                    (v.Email != null && v.Email.ToLower().Contains(search)) ||
                    (v.ContactNumber != null && v.ContactNumber.Contains(search))
                );
            }

            // Apply sort
            query = sort switch
            {
                "NameAsc" => query.OrderBy(v => v.FullName),
                "NameDesc" => query.OrderByDescending(v => v.FullName),
                "DateNewest" => query.OrderByDescending(v => v.TimeIn),
                "DateOldest" => query.OrderBy(v => v.TimeIn),
                "Status" => query.OrderBy(v => v.TimeOut.HasValue).ThenByDescending(v => v.TimeIn),
                _ => query.OrderByDescending(v => v.TimeIn)
            };

            return query;
        }
    }
}