using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Helpers;

public static class BookQueryHelper
{
    public static (IQueryable<BookViewModel> Books, int TotalPages) GetBestRatedBooks(ApplicationDbContext context, int page, int pageSize, string? userId = null)
    {
        int totalPages = GetTotalPages(context.Books, pageSize); 
            
        var books = context.Books
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return (books, totalPages);
    }

    public static (IQueryable<BookViewModel> Books, int TotalPages) GetSearchedBooks(ApplicationDbContext context, string searchQuery, int page, int pageSize,
        string? userId = null)
    {
        int totalPages = GetTotalPages(context.Books.Where(b =>
            b.Author.Name.Contains(searchQuery) || 
            b.Title.Contains(searchQuery) ||
            b.Isbn == searchQuery)
            , pageSize);
        var books = context.Books
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Where(b => b.Author.Name.Contains(searchQuery) || b.Title.Contains(searchQuery) ||
                        b.Isbn == searchQuery)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return (books, totalPages);
    }
    public static (IQueryable<BookViewModel> Books, int TotalPages) GetUserBooks(ApplicationDbContext context, ClaimsPrincipal User, int page, int pageSize)
    {
        string userId = User.GetUserId();
        
        int totalPages = GetTotalPages(context.UserLibraries.Where(u => u.UserId == userId), pageSize);
        IQueryable<BookViewModel> books = context.UserLibraries
            .Include(b => b.Book)
            .ThenInclude(b => b.UserLibraries.Where(u => u.UserId == userId))
            .Include(a => a.Book.Author)
            .Where(u => u.UserId == userId)
            .Select(book => MapToBookViewModel(book.Book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return (books, totalPages);

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

    private static int GetTotalPages<T>(IQueryable<T> dbSet, int pageSize) where T : class
    {
        int totalBooks = dbSet.Count();
        return (int)Math.Ceiling((float) totalBooks/ pageSize);
    }
    
}