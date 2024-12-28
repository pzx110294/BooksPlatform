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
            List<Book> books = db.Books
                .Include(a => a.Author)
                .Where(b => b.Rating < 5)
                .OrderByDescending(b => b.Rating)
                .Take(10)
                .ToList();
            ViewData["Header"] = "Best rated books";
            return View(books);
        }

    
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (String.IsNullOrEmpty(searchQuery))
            {
                return NotFound();
            }

            Console.WriteLine(searchQuery);
            List<Book> books = db.Books
                .Include(a => a.Author)
                .OrderByDescending(b => b.Rating)
                .Where(b => b.Author.Name.Contains(searchQuery) || b.Title.Contains(searchQuery) || b.Isbn == searchQuery)
                .Take(10)
                .ToList();
            ViewData["Header"] = "Searched books";
            return View(books);
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
