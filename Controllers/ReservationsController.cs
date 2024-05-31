using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Reservations
    public async Task<IActionResult> Index()
    {
        var reservations = _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Book);
        return View(await reservations.ToListAsync());
    }

    // GET: Reservations/Reserve
    public IActionResult Reserve()
    {
        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title");

        var viewModel = new ReservationViewModel
        {
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
                Status = "Reserved"
            };

            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title");
        return View(reservationViewModel);
    }

    // GET: Reservations/Issue/5
    public async Task<IActionResult> Issue(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(m => m.ReservationID == id);
        if (reservation == null)
        {
            return NotFound();
        }

        var viewModel = new IssueReservationViewModel
        {
            ReservationID = reservation.ReservationID,
            GuestID = reservation.GuestID,
            Guest = reservation.Guest,
            BookID = reservation.BookID,
            Book = reservation.Book,
            CDID = reservation.CDID,
            ReservationDate = reservation.ReservationDate,
            IssueDate = reservation.IssueDate,
            ReturnDate = reservation.ReturnDate,
            Status = reservation.Status
        };

        ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name", reservation.GuestID);
        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", reservation.BookID);

        return View(viewModel);
    }

    // POST: Reservations/Issue/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(int id, [Bind("ReservationID, GuestID, BookID, CDID, ReservationDate, IssueDate, ReturnDate, Status")] IssueReservationViewModel viewModel)
    {
        if (id != viewModel.ReservationID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(viewModel.ReservationID);
                if (reservation == null)
                {
                    return NotFound();
                }

                reservation.IssueDate = viewModel.IssueDate;
                reservation.ReturnDate = viewModel.ReturnDate;
                reservation.Status = viewModel.Status;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(viewModel.ReservationID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name", viewModel.GuestID);
        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", viewModel.BookID);
        return View(viewModel);
    }

    // GET: Reservations/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        var viewModel = new EditReservationViewModel
        {
            ReservationID = reservation.ReservationID,
            GuestID = reservation.GuestID,
            Guest = reservation.Guest,
            BookID = reservation.BookID,
            Book = reservation.Book,
            CDID = reservation.CDID,
            ReservationDate = reservation.ReservationDate,
            IssueDate = reservation.IssueDate,
            ReturnDate = reservation.ReturnDate,
            Status = reservation.Status
        };

        ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name", reservation.GuestID);
        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", reservation.BookID);
        return View(viewModel);
    }

    // POST: Reservations/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ReservationID, GuestID, BookID, CDID, ReservationDate, IssueDate, ReturnDate, Status")] EditReservationViewModel viewModel)
    {
        if (id != viewModel.ReservationID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(viewModel.ReservationID);
                if (reservation == null)
                {
                    return NotFound();
                }

                reservation.ReservationDate = viewModel.ReservationDate;
                reservation.IssueDate = viewModel.IssueDate;
                reservation.ReturnDate = viewModel.ReturnDate;
                reservation.Status = viewModel.Status;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(viewModel.ReservationID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name", viewModel.GuestID);
        ViewBag.Books = new SelectList(_context.Books, "BookID", "Title", viewModel.BookID);
        return View(viewModel);
    }



    // GET: Reservations/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(m => m.ReservationID == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // POST: Reservations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Reservations/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(m => m.ReservationID == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    private bool ReservationExists(int id)
    {
        return _context.Reservations.Any(e => e.ReservationID == id);
    }
}
