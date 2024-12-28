using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;

namespace Serwis_Książkowy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Index()
        {
            return View();
            List<Book> books = db.Books
                .Include(a => a.Author)
                .Where(b => b.Rating < 5)
                .OrderByDescending(b => b.Rating)
                .Take(10)
                .ToList();
            ViewData["Header"] = "Best rated books";
            return View(books);
        }

    
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
