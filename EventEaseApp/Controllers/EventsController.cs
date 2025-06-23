using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventEaseApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchType, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            var events = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
                events = events.Where(e => e.EventType.Name == searchType);

            if (venueId.HasValue)
                events = events.Where(e => e.VenueID == venueId);

            if (startDate.HasValue && endDate.HasValue)
                events = events.Where(e => e.EventDate >= startDate && e.EventDate <= endDate);

            // ✅ Set ViewData for dropdowns
            ViewData["EventType"] = _context.EventType.ToList();
            ViewData["Venues"] = _context.Venue.ToList();

            // Optional: remember filters
            ViewData["SelectedType"] = searchType;
            ViewData["SelectedVenue"] = venueId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(await events.ToListAsync());
        }

        public IActionResult Create()
        {
            //ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
            ViewBag.EventTypeID = new SelectList(_context.EventType, "EventTypeID", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event ctreated successfully.";
                return RedirectToAction(nameof(Index));

            }
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            ViewBag.EventTypeID = new SelectList(_context.EventType, "EventTypeID", "Name");

            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Events.Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.EventID == id);

            if (events == null)
            {

                return NotFound();

            }
            return View(events);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Events.Include(m => m.Venue).FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
            {

                return NotFound();

            }
            return View(events);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           
                var events = await _context.Events.FindAsync(id);
            
            if (events == null) return NotFound();

            var isBooked = await _context.Booking.AnyAsync(b => b.EventID == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has an existing booking.";
                return RedirectToAction(nameof(Index));
            }
                _context.Remove(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            // ✅ Repopulate dropdown
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            return View(events);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events events)
        {
            if (id != events.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {    
                _context.Update(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
             
            }
            // ✅ Repopulate dropdown again in case of error
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            return View(events);
        }
    }
}

