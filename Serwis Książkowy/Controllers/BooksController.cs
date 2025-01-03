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
            string userId = User.GetUserId();
            
            var results = BookQueryHelper.GetBestRatedBooks(_context, page, pageSize, userId);
            IQueryable<BookViewModel> bookViewModel = results.Books;
            
            ViewData["Header"] = "Best rated books";
            SetPaginationData(page, results.TotalPages);
            return View(bookViewModel);
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
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

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
                if (!BookQueryHelper.BookExists(book.Isbn, _context))
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                // Log ModelState errors for debugging
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        var errors = state.Errors.Select(e => e.ErrorMessage);
                        Console.WriteLine($"Key: {key}, Errors: {string.Join(", ", errors)}");
                    }
                }
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorId", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", book.GenreId);
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
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorId", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", book.GenreId);
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

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
