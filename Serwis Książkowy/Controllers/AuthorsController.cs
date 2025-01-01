using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;

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

        [HttpPost]
        public IActionResult Follow(int authorId)
        {
            string userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return NotFound();

            FavouriteAuthor favouriteAuthor = new FavouriteAuthor
            {
                AuthorId = authorId,
                UserId = userId
            };
            _context.FavouriteAuthors.Add(favouriteAuthor);
            _context.SaveChanges();
            
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }
        
    }
}
