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
            .Include(g => g.Genre)
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
            b.Title.Contains(searchQuery) ||
            b.Isbn == searchQuery);
        
        int totalPages = GetTotalPages(query, pageSize);
        
        var books = query
            .Include(a => a.Author)
            .Include(g => g.Genre)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return (books, totalPages);
    }
    public static (IQueryable<AuthorViewModel> Authors, int TotalPages) GetSearchedAuthors(
        ApplicationDbContext context, string searchQuery, int page, int pageSize,
        string? userId = null)
    {
        IQueryable<Author> query = context.Authors
            .Where(a => a.Name.Contains(searchQuery));
    
        int totalPages = GetTotalPages(query, pageSize);
    
        var authors = query
            .Select(author => new AuthorViewModel
            {
                Author = author,
                IsFollowed = userId != null && context.FavouriteAuthors.Any(fa => fa.UserId == userId && fa.AuthorId == author.AuthorId)
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    
        return (authors, totalPages);
    }
    public static (IQueryable<BookViewModel> Books, int TotalPages) GetUserBooks(
        ApplicationDbContext context, string userId, int page, int pageSize)
    {
        IQueryable<UserLibrary> query = context.UserLibraries
            .Where(u => u.UserId == userId);
        
        int totalPages = GetTotalPages(query, pageSize);
        page = CheckPageBounds(page, totalPages);
        IQueryable<BookViewModel> books = query
            .Include(b => b.Book)
            .ThenInclude(b => b.UserLibraries.Where(u => u.UserId == userId))
            .Include(g => g.Book.Genre)
            .Include(a => a.Book.Author)
            .Select(book => MapToBookViewModel(book.Book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return (books, totalPages);
    }

    private static int CheckPageBounds(int page, int totalPages)
{
    if (totalPages == 0)
    {
        return 1;
    }
    if (page > totalPages)
    {
        return totalPages;
    }
    return page;
}

    public static (IQueryable<BookViewModel> Books, int TotalPages, bool isFollowed) GetAuthorBooks(
        ApplicationDbContext context, int authorId, int page, int pageSize, string? userId = null)
    {
        IQueryable<Book> query = context.Books.Where(b => b.AuthorId == authorId);
        int totalPages = GetTotalPages(query, pageSize);
        IQueryable<BookViewModel> books = query
            .Include(a => a.Author)
            .Include(l => l.UserLibraries)
            .Include(g => g.Genre)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        bool isFollowed = context.FavouriteAuthors.Any(u => u.UserId == userId && u.AuthorId == authorId);
        
        return (books, totalPages, isFollowed);
    }
    private static int GetTotalPages<T>(IQueryable<T> dbSet, int pageSize) where T : class
    {
        int totalBooks = dbSet.Count();
        return (int)Math.Ceiling((float) totalBooks/ pageSize);
    }

    public static (IEnumerable<AuthorBookViewModel> authors, int TotalPages) GetRecentBooksFromFavouriteAuthors(ApplicationDbContext context, string userId, int page, int pageSize)
    {
        IQueryable<FavouriteAuthor> query = context.FavouriteAuthors
            .Where(fa => fa.UserId == userId);
        int totalPages = GetTotalPages(query, pageSize);
        page = CheckPageBounds(page, totalPages);

        var authors = query
            .Select(a => new AuthorBookViewModel
            {
                AuthorViewModel = new AuthorViewModel
                {
                    Author = a.Author,
                    IsFollowed = true
                },
                BookViewModel = context.Books
                    .Where(book => book.AuthorId == a.Author.AuthorId)
                    .Include(ul => ul.UserLibraries)
                    .Include(g => g.Genre)
                    .OrderByDescending(b => b.PublicationDate)
                    .Select(b => BookQueryHelper.MapToBookViewModel(b, userId))
                    .Take(1)
                    .ToList()
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return (authors, totalPages);
    }

    private static BookViewModel MapToBookViewModel(Book book, string? userId)
    {
        return new BookViewModel
        {
            Book = book,
            IsInLibrary = userId != null && book.UserLibraries.Any(u => u.UserId == userId),
            Status = userId != null
                ? book.UserLibraries
                    .Where(u => u.UserId == userId)
                    .Select(u => u.Status)
                    .FirstOrDefault()
                : Status.Completed
        };
    }

    public static bool BookExists(string isbn, ApplicationDbContext context)
    {
        return context.Books.Any(b => b.Isbn == isbn);
    }

    public static (IQueryable<BookViewModel> Books, int TotalPages) GetGenreBooks(ApplicationDbContext context,
        int genreId, string userId, int page, int pageSize)
    {
        IQueryable<Book> query = context.Books
            .Where(u => u.GenreId == genreId);
        
        int totalPages = GetTotalPages(query, pageSize);
        
        IQueryable<BookViewModel> books = query
            .Include(a => a.Author)
            .Include(g => g.Genre)
            .Include(l => l.UserLibraries.Where(u => u.UserId == userId))
            .OrderByDescending(b => b.Rating)
            .Select(book => MapToBookViewModel(book, userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return (books, totalPages);
    }
}