using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookThru.Models;
using Microsoft.EntityFrameworkCore;

namespace BookThru.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookThruContext _context;
        // private  BookThruContext db= new BookThruContext();
        public HomeController(BookThruContext context)
        {
            _context = context;
        }

        
      
        public IActionResult Index()
        {
            
   
            return View(_context.UserInfo.ToList());
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
