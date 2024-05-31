using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class GuestsController : Controller
{
    private readonly ApplicationDbContext _context;

    public GuestsController(ApplicationDbContext context)
    {
        _context = context;
    }
    // Leave a message
    public IActionResult LeaveMessage()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LeaveMessage([Bind("GuestID, MessageText")] Message message)
    {
        if (ModelState.IsValid)
        {
            _context.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(message);
    }

    // Create Guest and Leave a Message
    public IActionResult CreateGuestWithMessage()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGuestWithMessage(CreateGuestWithMessageVm vm)
    {
        if (ModelState.IsValid)
        {
            _context.Guests.Add(vm.Guest);
            await _context.SaveChangesAsync();

            vm.Message.GuestID = vm.Guest.GuestID;
            _context.Messages.Add(vm.Message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View(vm);
    }
}
