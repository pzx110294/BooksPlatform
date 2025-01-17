using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Controllers
{
    public class BooksController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Best rated books
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            string userId = User.GetUserId();
            var results = BookQueryHelper.GetBestRatedBooks(_context, page, pageSize, userId);
            IQueryable<BookViewModel> bookViewModel = results.Books;
            
            ViewData["Header"] = "Best rated books";
            SetPaginationData(page, results.TotalPages);
            return View(bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
        public IActionResult AddReview(int bookId, string reviewText, float rating)
        {
            string userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrWhiteSpace(reviewText) || reviewText.Length > 10 || rating < 0 || rating > 5)
            {
                TempData["ReviewErrors"] = new[] { "Invalid review input." };
                TempData["ReviewText"] = reviewText;
                return RedirectToAction(nameof(Details), new { id = bookId });
            }
            
            bool alreadyReviewed = _context.Reviews.Any(r => r.UserId == userId && r.BookId == bookId);
            if (alreadyReviewed)
            {
                TempData["ReviewErrors"] = new[] { "You have already reviewed this book." };
                return RedirectToAction(nameof(Details), new { id = bookId });
            }

            // Create and save the review
            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                ReviewText = reviewText,
                Rating = rating,
                CreatedAt = DateTime.Today
            };

            _context.Reviews.Add(review);
            BookQueryHelper.UpdateBookRating(_context, bookId);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = bookId });
        }


        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            string userId = User.GetUserId();
            ViewBag.Reviewed = book.Reviews.Any(r => r.UserId == userId);
            return View(book);
        }

        // GET: Books/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("BookId,Isbn,Title,AuthorId,GenreId,PublicationDate,Rating")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (!BookExists(book.Isbn))
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "A book with this ISBN already exists.");
                }
            }
            
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }
        

        [Authorize (Roles = "Admin")]
        public IActionResult ListBooks(int page = 1, int pageSize = 30)
        {
            var books = _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            int totalBooks = _context.Books.Count();
            int totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            SetPaginationData(page, totalPages);

            return View(books);
        }
        // GET: Books/Edit/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Isbn,Title,AuthorId,GenreId,PublicationDate,Rating")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Details), new { id = id});
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string isbn)
        {
            return _context.Books.Any(e => e.Isbn == isbn);
        }
    }
}
