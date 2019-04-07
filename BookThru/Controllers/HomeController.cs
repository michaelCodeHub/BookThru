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

        public IActionResult Create(Book book)
        {

            try
            {
                _context.Book.Add(book);
                _context.SaveChanges();
                return RedirectToAction("showBooks");

            }

            catch
            {
                //return View();
            }
         
            return RedirectToAction("Index");

            //return View();
        }
        public IActionResult CreateUser(UserInfo addUser)
        {
              _context.UserInfo.Add(addUser);
                _context.SaveChanges();
                //RedirectToAction("Index");
            return View();
        }
        public IActionResult Index()
        {
            var userInformation = _context.UserInfo.ToList();
            return View(userInformation);
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
        public IActionResult About()
        {
            return View(_context.UserInfo.ToList());
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

        public IActionResult showBooks() {

            return View(_context.Book.ToList());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("BookId, Name,CategoryID,CourseCodeID,Editon,Description,MinimumBidePrice,BuyNowPrice,Message,Status")] Book book)
        {

            _context.Entry(book).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }


    }
}
