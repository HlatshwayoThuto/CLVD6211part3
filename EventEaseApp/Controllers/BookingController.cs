using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventEaseApp.Controllers
{

    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var booking = _context.Booking.Include(b => b.Venue).Include(b => b.Events).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                booking = booking.Where(b => 
                b.Venue.VenueName.Contains(searchString) ||
                b.Events.EventName.Contains(searchString));
            }

            return View(await booking.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//new part
        public async Task<IActionResult> Create(Booking booking)
        {
            var selectedEvent = await _context.Events.FirstOrDefaultAsync(equals => equals.EventID == booking.EventID);//new part

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found");
                ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
                ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
                return View(booking);


            }

            var conflict = await _context.Booking
                .Include(b => b.Events)
                .AnyAsync(b => b.VenueID == booking.VenueID
                && b.Events.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for this date.");
                ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
                ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
                return View(booking);
            }

            if (ModelState.IsValid)
            {

                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
                    ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
                    return View(booking);
                }

            }

            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
            return View(booking);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Events)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking.Include(b => b.Events)
                .Include(b => b.Venue).FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {

                return NotFound();

            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            // ✅ Repopulate dropdowns
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", booking.VenueID);
            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            return View(booking);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking updated successfully.";
                return RedirectToAction(nameof(Index));
            }
                // ✅ Repopulate dropdowns again in case of form error
                ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", booking.VenueID);
                ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
                return View(booking);
            }
        }
    }


