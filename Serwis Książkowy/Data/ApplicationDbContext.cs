using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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


        modelBuilder.Entity<UserLibrary>()
            .HasKey(ul => new { ul.UserId, ul.BookId });
        modelBuilder.Entity<UserLibrary>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.UserLibraries)
            .HasForeignKey(ul => ul.UserId);
        modelBuilder.Entity<UserLibrary>()
            .HasOne(ul => ul.Book)
            .WithMany(b => b.UserLibraries)
            .HasForeignKey(ul => ul.BookId);
        modelBuilder.Entity<UserLibrary>()
            .Property(ul => ul.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);

        modelBuilder.Entity<FavouriteAuthor>()
            .HasKey(fa => new { fa.UserId, fa.AuthorId });
        modelBuilder.Entity<FavouriteAuthor>()
            .HasOne(fa => fa.User)
            .WithMany(u => u.FavouriteAuthors)
            .HasForeignKey(fa => fa.UserId);
        modelBuilder.Entity<FavouriteAuthor>()
            .HasOne(fa => fa.Author)
            .WithMany(a => a.FavouriteAuthors)
            .HasForeignKey(fa => fa.AuthorId);


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