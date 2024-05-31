using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DWD_CW_Final.Data;
using DWD_CW_Final.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DWD_CW_Final.Controllers
{
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult SignIn()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignIn(Admin admin)
		{
			if (ModelState.IsValid)
			{
				var existingAdmin = await _context.Admins
					.FirstOrDefaultAsync(a => a.Username == admin.Username && a.Password == admin.Password);
				if (existingAdmin != null)
				{
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, existingAdmin.Username),
						new Claim(ClaimTypes.Role, "Admin")
					};

					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var authProperties = new AuthenticationProperties
					{
						IsPersistent = true
					};

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
						new ClaimsPrincipal(claimsIdentity), authProperties);

					// Redirect to the admin dashboard
					return RedirectToAction("Index", "Dashboard");
				}
				ModelState.AddModelError("", "Invalid username or password");
			}
			return View(admin);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			HttpContext.Session.Clear();
			return RedirectToAction("SignIn");
		}
	}
}
