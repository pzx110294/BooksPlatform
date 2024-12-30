using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;

namespace Serwis_Książkowy.Controllers
{
    public class UserLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserLibrary
        public ActionResult Index()
        {
            var books = BookQueryHelper.GetUserBooks(_context, User);
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