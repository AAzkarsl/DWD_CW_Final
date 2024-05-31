using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search)
    {
        ViewData["search"] = search;

        var books = from b in _context.Books
                    select b;

        if (!string.IsNullOrEmpty(search))
        {
            books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        }

        return View(await books.ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // GET: Home/Reserve/5
    public async Task<IActionResult> Reserve(int? bookId)
    {
        if (bookId == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound();
        }

        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", bookId);

        var viewModel = new ReservationViewModel
        {
            BookID = book.BookID,
            ReservationDate = DateTime.Today
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve([Bind("GuestName, GuestEmail, BookID, CDID, ReservationDate")] ReservationViewModel reservationViewModel)
    {
        if (ModelState.IsValid)
        {
            var guest = new Guest
            {
                Name = reservationViewModel.GuestName,
                Email = reservationViewModel.GuestEmail
            };
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                GuestID = guest.GuestID,
                BookID = reservationViewModel.BookID,
                CDID = reservationViewModel.CDID,
                ReservationDate = reservationViewModel.ReservationDate,
                Status = "Reserved" // Set default status to "Reserved"
            };

            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", reservationViewModel.BookID);
        return View(reservationViewModel);
    }
}
