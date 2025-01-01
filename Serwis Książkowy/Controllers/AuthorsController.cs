using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;

namespace Serwis_Książkowy.Controllers
{
    public class AuthorsController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: AuthorsController
        public ActionResult Index(int authorId, int page = 1, int pageSize = 10)
        {
            string userId = User.GetUserId();
            var results = BookQueryHelper.GetAuthorBooks(_context, authorId, page, pageSize, userId);

            SetPaginationData(page, results.TotalPages);
            ViewData["Header"] = "Books by " + results.Books.FirstOrDefault()?.Book.Author.Name;
            return View(results.Books);
        }

    
    }
}
