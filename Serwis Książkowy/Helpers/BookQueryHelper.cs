using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;
using Serwis_Książkowy.ViewModels;

namespace Serwis_Książkowy.Helpers;

public static class BookQueryHelper
{
    public static (IQueryable<BookViewModel> Books, int TotalPages) GetBestRatedBooks(
        ApplicationDbContext context, int page, int pageSize, string? userId = null)
    {
        IQueryable<Book> query = context.Books;
        int totalPages = GetTotalPages(query, pageSize); 
            
        var books = query
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return (books, totalPages);
    }

    public static (IQueryable<BookViewModel> Books, int TotalPages) GetSearchedBooks(
        ApplicationDbContext context, string searchQuery, int page, int pageSize,
        string? userId = null)
    {
        IQueryable<Book> query = context.Books
            .Where(b =>
            b.Author.Name.Contains(searchQuery) ||
            b.Title.Contains(searchQuery) ||
            b.Isbn == searchQuery);
        
        int totalPages = GetTotalPages(query, pageSize);
        
        var books = query
            .Include(a => a.Author)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return (books, totalPages);
    }
    public static (IQueryable<BookViewModel> Books, int TotalPages) GetUserBooks(
        ApplicationDbContext context, ClaimsPrincipal User, int page, int pageSize)
    {
        string userId = User.GetUserId();

        IQueryable<UserLibrary> query = context.UserLibraries
            .Where(u => u.UserId == userId);
        
        int totalPages = GetTotalPages(query, pageSize);
        
        IQueryable<BookViewModel> books = query
            .Include(b => b.Book)
            .ThenInclude(b => b.UserLibraries.Where(u => u.UserId == userId))
            .Include(a => a.Book.Author)
            .Select(book => MapToBookViewModel(book.Book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return (books, totalPages);
    }

    public static (IQueryable<BookViewModel> Books, int TotalPages) GetAuthorBooks(
        ApplicationDbContext context, int authorId, int page, int pageSize, string? userId = null)
    {
        IQueryable<Book> query = context.Books.Where(b => b.AuthorId == authorId);
        int totalPages = GetTotalPages(query, pageSize);

        IQueryable<BookViewModel> books = query
            .Include(a => a.Author)
            .Include(l => l.UserLibraries)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return (books, totalPages);
    }
    private static int GetTotalPages<T>(IQueryable<T> dbSet, int pageSize) where T : class
    {
        int totalBooks = dbSet.Count();
        return (int)Math.Ceiling((float) totalBooks/ pageSize);
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