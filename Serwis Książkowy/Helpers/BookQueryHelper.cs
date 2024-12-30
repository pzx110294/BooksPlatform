using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Helpers;

public static class BookQueryHelper
{
    public static IQueryable<BookViewModel> GetBestRatedBooks(ApplicationDbContext context, string? userId = null)
    {
        return context.Books
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .Where(b => b.Rating < 5)
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Take(10);
    }

    public static IQueryable<BookViewModel> GetSearchedBooks(ApplicationDbContext context, string searchQuery,
        string? userId = null)
    {
        return context.Books
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Where(b => b.Author.Name.Contains(searchQuery) || b.Title.Contains(searchQuery) ||
                        b.Isbn == searchQuery)
            .Select(book => MapToBookViewModel(book, userId))
            .Take(10);
    }
    public static IQueryable<BookViewModel> GetUserBooks(ApplicationDbContext context, ClaimsPrincipal User)
    {
        string userId = User.GetUserId();

        IQueryable<BookViewModel> library = context.UserLibraries
            .Include(b => b.Book)
            .ThenInclude(b => b.UserLibraries.Where(u => u.UserId == userId))
            .Include(a => a.Book.Author)
            .Where(u => u.UserId == userId)
            .Select(book => MapToBookViewModel(book.Book, userId));

        return library;
    }

    private static BookViewModel MapToBookViewModel(Book book, string? userId)
    {
        return new BookViewModel
        {
            Book = book,
            IsInLibrary = userId != null && book.UserLibraries.Any(),
            Status = userId != null
                ? book.UserLibraries
                    .Where(u => u.UserId == userId)
                    .Select(u => u.Status)
                    .FirstOrDefault()
                : Status.Completed
        };
    }

}