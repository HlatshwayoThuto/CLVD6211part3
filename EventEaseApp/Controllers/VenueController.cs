using Azure.Core;
using Azure.Storage.Blobs;
using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var venue = await _context.Venue.ToListAsync();
            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {

            
            if (ModelState.IsValid)
            {


                //handle image upload to azure blob storage if an image file was provided
                //This is step4: modify controller to recieve imige file from view(use upload)
                //This is step5: upload selected image to azure blob storage
                if(venue.ImageFile != null)
                {
                    //upload image to blob storage(azure)
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);//part of step 5

                    //step 6: save blob url into imageUrl property
                    venue.ImageUrl = blobUrl;
                }
                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully";
                return RedirectToAction(nameof(Index));

            }
            return View(venue);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null) return NotFound();

            return View(venue);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
            {

                return NotFound();

            }
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var venue = await _context.Venue.FindAsync(id);
    if (venue == null) return NotFound();

    // Check for Events related to this Venue
    var hasEvents = await _context.Events.AnyAsync(e => e.VenueID == id);
    if (hasEvents)
    {
        var eventsWithBookings = await _context.Events
            .Where(e => e.VenueID == id)
            .AnyAsync(e => _context.Booking.Any(b => b.EventID == e.EventID));

        if (eventsWithBookings)
        {
            TempData["ErrorMessage"] = "Cannot delete Venue because one or more associated Events have existing Bookings.";
            return RedirectToAction(nameof(Index));
        }
    }

    _context.Venue.Remove(venue);
    await _context.SaveChangesAsync();
    TempData["successMessage"] = "Venue deleted successfully.";
    return RedirectToAction(nameof(Index));
}


        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        //upload new image if provided
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        //update Venue.imageUrl with the new blob URL
                        venue.ImageUrl = blobUrl;
                    }
                    else
                    {
                        //keep the existing Url(optional depending on the UI design)
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";


                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!VenueExists(venue.VenueID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));

            }
            return View(venue);
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=practiceimg;AccountKey=z6V4vTv1NpstjAzU+u/IxtcWiKg2hb4BJZgB7/Q/wyfNdztTq5AKCa26/WBvejTdF82gYomgjuh2+AStuNP6Yw==;EndpointSuffix=core.windows.net";
            var containerName = "part2";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

            }
            return blobClient.Uri.ToString();
        }
    }
}

