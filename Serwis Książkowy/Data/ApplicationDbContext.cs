using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data.Configurations;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors  { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Review> Reviews  { get; set; }
    public DbSet<UserLibrary> UserLibraries  { get; set; }
    public DbSet<FavouriteAuthor> FavouriteAuthors  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureModel();
        
        modelBuilder.Entity<Genre>().HasData(
            new Genre { GenreId = 1, Name = "Fantasy" },
            new Genre { GenreId = 2, Name = "Science Fiction" },
            new Genre { GenreId = 3, Name = "Mystery" },
            new Genre { GenreId = 4, Name = "Non-Fiction" },
            new Genre { GenreId = 5, Name = "Romance" }
        );
        modelBuilder.Entity<Author>().HasData(
            new Author {AuthorId = 1, Name = "Pierce Brosnan"}
        );
        modelBuilder.Entity<Book>().HasData(
            new Book { BookId = 1, Title = "Book1", AuthorId = 1, GenreId = 3, PublicationDate = DateTime.Today }); }
    
}