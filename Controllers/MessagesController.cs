using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

public class MessagesController : Controller
{
	private readonly ApplicationDbContext _context;

	public MessagesController(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		return View(await _context.Messages.Include(m => m.Guest).ToListAsync());
	}

	public IActionResult Add()
	{
		ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name");
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Add([Bind("GuestID, MessageText, MessageDate")] Message message)
	{
		if (ModelState.IsValid)
		{
			_context.Add(message);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		ViewBag.Guests = new SelectList(_context.Guests, "GuestID", "Name", message.GuestID);
		return View(message);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var message = await _context.Messages
			.Include(m => m.Guest)
			.FirstOrDefaultAsync(m => m.MessageID == id);
		if (message == null)
		{
			return NotFound();
		}

		return View(message);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var message = await _context.Messages.FindAsync(id);
		_context.Messages.Remove(message);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool MessageExists(int id)
	{
		return _context.Messages.Any(e => e.MessageID == id);
	}

	// Leave a message
	public IActionResult LeaveMessage()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LeaveMessage(CreateGuestWithMessageVm vm)
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
