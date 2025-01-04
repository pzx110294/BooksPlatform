using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Helpers;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Controllers
{
    public class GenresController : PaginationController
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ListBooksByGenre(int genreId, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            var userId = User.GetUserId();
            var results = BookQueryHelper.GetGenreBooks(_context, genreId, userId, page, pageSize);
            IQueryable<BookViewModel> books = results.Books;
            int totalPages = results.TotalPages;
            SetPaginationData(page, totalPages);

            ViewData["GenreId"] = genreId;
            ViewData["Header"] = _context.Genres.Find(genreId)?.Name;
            return View(books);
        }
        // GET: Genres
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }
        // GET: Genres/Details/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("GenreId,Name")] Genre genre)
        {
            if (!ModelState.IsValid) return View(genre);
            if (GenreExists(genre.Name))
            {
                ModelState.AddModelError(string.Empty, "Genre with this name already exists.");
                return View(genre);
            }
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Genres/Edit/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name")] Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!GenreExists(genre.Name)){
                        _context.Update(genre);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Genre with this name already exists.");
                        return View(genre);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreId))
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
            return View(genre);
        }

        // GET: Genres/Delete/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }
        private bool GenreExists(string name)
        {
            return _context.Genres.Any(e => e.Name == name);
        }
    }
}
