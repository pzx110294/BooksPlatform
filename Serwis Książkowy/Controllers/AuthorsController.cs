using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Controllers
{
    public class AuthorsController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(int authorId, int page = 1, int pageSize = 10)
        {
            string userId = User.GetUserId();
            var results = BookQueryHelper.GetAuthorBooks(_context, authorId, page, pageSize, userId);

            SetPaginationData(page, results.TotalPages);
            ViewData["Header"] = "Books by " + results.Books.FirstOrDefault()?.Book.Author.Name;

            AuthorViewModel authorViewModel = new AuthorViewModel
            {
                Author = results.Books.FirstOrDefault().Book.Author,
                IsFollowed = results.isFollowed
            };
            AuthorBookViewModel viewModel = new AuthorBookViewModel
            {
                AuthorViewModel = authorViewModel,
                BookViewModel = results.Books
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Follow(int authorId)
        {
            string userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return NotFound();

            bool isAlreadyFavourited =
                _context.FavouriteAuthors.Any(fa => fa.UserId == userId && fa.AuthorId == authorId);
            if (!isAlreadyFavourited)
            {
                return NotFound();
            }
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
        public IActionResult Unfollow(int authorId)
        {
            string userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return NotFound();

            FavouriteAuthor? favouriteAuthor = _context.FavouriteAuthors.Find(userId, authorId);
            if (favouriteAuthor != null) _context.FavouriteAuthors.Remove(favouriteAuthor);
            _context.SaveChanges();
            
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }

        public IActionResult FollowedAuthors()
        {
            string userId = User.GetUserId();
            IQueryable<AuthorViewModel> favouriteAuthors = _context.FavouriteAuthors
                .Where(u => u.UserId == userId)
                .Select(a => new AuthorViewModel
                {
                    Author = a.Author,
                    IsFollowed = true
                });
            
            return View(favouriteAuthors);
        }

    }
}
