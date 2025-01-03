using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Models;
using System.Diagnostics;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Controllers
{
    public class HomeController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

    
        public IActionResult Search(string searchQuery, int page = 1, int pageSize = 10)
        {
            if (String.IsNullOrEmpty(searchQuery))
            {
                return NotFound();
            }
            page = page < 1 ? 1 : page;

            string userId = User.GetUserId();
            var results = BookQueryHelper.GetSearchedBooks(_context, searchQuery, page, pageSize, userId);
            IQueryable<BookViewModel> bookViewModel = results.Books;
            int totalPages = results.TotalPages;
            SetPaginationData(page, totalPages);
            ViewData["SearchQuery"] = searchQuery;
            ViewData["Header"] = $"Search results for \"{searchQuery}\"";
            return View(bookViewModel);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
