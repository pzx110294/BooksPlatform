using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Controllers
{
    public class UserLibraryController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public UserLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserLibrary
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            var userId = User.GetUserId();
            var results = BookQueryHelper.GetUserBooks(_context, userId, page, pageSize);
            IQueryable<BookViewModel> books = results.Books;
            int totalPages = results.TotalPages;
            SetPaginationData(page, totalPages);
            string emptyLibrary = books.IsNullOrEmpty() ? " is empty" : String.Empty;
            ViewData["Header"] = "Your library" + emptyLibrary;
            return View(books);
        }

        [HttpPost]
        public IActionResult AddToLibrary(int bookId, Status status)
        {
            var userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return NotFound();

            var user = _context.Users.Include(u => u.UserLibraries).FirstOrDefault(u => u.Id == userId);
            var book = _context.Books.Include(b => b.UserLibraries).FirstOrDefault(b => b.BookId == bookId);
            if (user == null || book == null)
            {
                return NotFound();
            }

            UserLibrary? existingUserLibrary = _context.UserLibraries
                .FirstOrDefault(ul => ul.UserId == userId && ul.BookId == bookId);

            if (existingUserLibrary != null)
            {
                existingUserLibrary.Status = status;
                existingUserLibrary.AddedAt = DateTime.Now;
            }
            else
            {
                UserLibrary userLibrary = new UserLibrary
                {
                    UserId = userId,
                    BookId = bookId,
                    Status = status,
                    AddedAt = DateTime.Now,
                    User = user,
                    Book = book
                };
                user.UserLibraries.Add(userLibrary);
                book.UserLibraries.Add(userLibrary);
                _context.UserLibraries.Add(userLibrary);
            }

            _context.SaveChanges();

            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }

        [HttpPost]
        public IActionResult DeleteFromLibrary(int bookId)
        {
            string userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return NotFound();

            var bookInLibrary = _context.UserLibraries.Find(userId, bookId);
            if (bookInLibrary != null) _context.UserLibraries.Remove(bookInLibrary); 
            _context.SaveChanges();
            
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }
    }
}