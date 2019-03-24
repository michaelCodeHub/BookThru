using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookThru.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookThru.Data;

namespace BookThru.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookThruContext _context;
        private readonly UserManager<BookThruUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        // private  BookThruContext db= new BookThruContext();
        public HomeController(BookThruContext context, UserManager <BookThruUser> userManager,
            RoleManager <IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
      
        public IActionResult Index()
        {
            return View(_context.UserInfo.ToList());
        }
        public async Task<IActionResult> ChangeRole(string Id)
        {
            var user = await _context.Users.FindAsync(Id);
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            if (roles.Contains("Admin"))
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(String Id)
        {
            BookThruContext dContext = new BookThruContext();

            if (Id == null)
            {
                return NotFound();
            }

            var user = _userManager.Users.Where(a => a.Id == Id).FirstOrDefault();
            UserInfo ur = new UserInfo { Id = user.Id, FirstName = user.UserName, LastName= user.UserName, PhoneNumber= user.PhoneNumber };
            var x = await _userManager.GetRolesAsync(user);
           // ur.R = String.Join(",", x);
            return View(ur);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
    }
}
