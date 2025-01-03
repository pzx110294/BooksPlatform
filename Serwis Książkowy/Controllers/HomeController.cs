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

        public IActionResult Search(string searchQuery, string searchType, int page = 1, int pageSize = 10)
        {
            if (String.IsNullOrEmpty(searchQuery))
            {
                return NotFound();
            }
            page = page < 1 ? 1 : page;

            string userId = User.GetUserId();
            IQueryable<BookViewModel> bookResults = null;
            IQueryable<AuthorViewModel> authorResults = null;
            int totalPages = 0;

            if (searchType == "book")
            {
                var results = BookQueryHelper.GetSearchedBooks(_context, searchQuery, page, pageSize, userId);
                bookResults = results.Books;
                totalPages = results.TotalPages;
                ViewData["Header"] = $"Search results for books: \"{searchQuery}\"";
            }
            else if (searchType == "author")
            {
                var results = BookQueryHelper.GetSearchedAuthors(_context, searchQuery, page, pageSize, userId);
                authorResults = results.Authors;
                totalPages = results.TotalPages;
                ViewData["Header"] = $"Search results for authors: \"{searchQuery}\"";
            }

            SetPaginationData(page, totalPages);
            ViewData["SearchQuery"] = searchQuery;
            ViewData["SearchType"] = searchType;

            var searchView = new AuthorBookSearchResultsView
            {
                AuthorViewModels = authorResults,
                BookViewModels = bookResults
            };
            
            return View("SearchResults", searchView);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
