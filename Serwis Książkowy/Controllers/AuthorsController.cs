using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (authorId == 0)
            {
                return NotFound();
            }
            page = page < 1 ? 1 : page;
            string userId = User.GetUserId();
            var results = BookQueryHelper.GetAuthorBooks(_context, authorId, page, pageSize, userId);

            var author = _context.Authors.FirstOrDefault(a => a.AuthorId == authorId);
            if (author == null)
            {
                return NotFound();
            }
        
            SetPaginationData(page, results.TotalPages);
            ViewData["Header"] = "Books by " + author.Name;

            AuthorViewModel authorViewModel = new AuthorViewModel
            {
                Author = author,
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
        [Authorize(Roles = "User, Admin")]
        public IActionResult Follow(int authorId)
        {
            string referer = Request.Headers["Referer"].ToString();
            
            string userId = User.GetUserId();
            if (String.IsNullOrEmpty(userId)) return Redirect(referer);

            bool isAlreadyFavourited =
                _context.FavouriteAuthors.Any(fa => fa.UserId == userId && fa.AuthorId == authorId);
            if (isAlreadyFavourited)
            {
                return Redirect(referer);
            }
            FavouriteAuthor favouriteAuthor = new FavouriteAuthor
            {
                AuthorId = authorId,
                UserId = userId
            };
            _context.FavouriteAuthors.Add(favouriteAuthor);
            _context.SaveChanges();
            
            
            return Redirect(referer);
        }
        [Authorize(Roles = "User, Admin")]
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

        [Authorize(Roles = "User, Admin")]
        public IActionResult FollowedAuthors(int page = 1, int pageSize = 10)
        {
            string userId = User.GetUserId();

            var results = BookQueryHelper.GetRecentBooksFromFavouriteAuthors(_context, userId, page, pageSize);
            
            SetPaginationData(page, results.TotalPages);
            ViewData["Header"] = "Recent books by followed authors";
            return View(results.authors);
        }

        // GET: Authors/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("AuthorId,Name")] Author author)
        {
            if (!ModelState.IsValid) return View(author);
            if (AuthorExists(author.Name))
            {
                ModelState.AddModelError(string.Empty, "Author with this name already exists.");
                return View(author);
            }
            _context.Add(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Books");
        }


        [Authorize (Roles = "Admin")]
        public IActionResult ListAuthors(int page = 1, int pageSize = 30)
        {
            var authors = _context.Authors
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            int totalAuthors = _context.Authors.Count();
            int totalPages = (int)Math.Ceiling(totalAuthors / (double)pageSize);

            SetPaginationData(page, totalPages);

            return View(authors);
        }
        // GET: Authors/Details/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,Name")] Author author)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!AuthorExists(author.Name))
                    {
                        _context.Update(author);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Author with this name already exists.");
                        return View(author);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new {authorId = author.AuthorId});
            }
            return View(author);
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.AuthorId == id);
        }
        private bool AuthorExists(string authorName)
        {
            return _context.Authors.Any(e => e.Name == authorName);
        }
        
        // GET: Authors/Delete/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), "Books");
        }
    }
}


