using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;



public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }




    [Route("admin")]
    [Authorize(Roles = "Admin")]
    // Add Book
    public IActionResult AddBook()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook([Bind("Title,Author,ISBN,Available")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewBooks));
        }
        return View(book);
    }

    // Delete Book
    public async Task<IActionResult> DeleteBook(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.BookID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("DeleteBook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBookConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ViewBooks));
    }

    // Edit Book
    public async Task<IActionResult> EditBook(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBook(int id, [Bind("BookID,Title,Author,ISBN,Available")] Book book)
    {
        if (id != book.BookID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.BookID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ViewBooks));
        }
        return View(book);
    }

    // View all books
    public async Task<IActionResult> ViewBooks()
    {
        return View(await _context.Books.ToListAsync());
    }

    // Add similar methods for CDs
    public IActionResult AddCD()
    {
        return View();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookID == id);
    }

}
