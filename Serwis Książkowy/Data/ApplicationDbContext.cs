using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Data.Configurations;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
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
        AddGenres(modelBuilder);
        
    }
    private void AddGenres(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasData(
            new Genre { GenreId = 1, Name = "Fiction" },
            new Genre { GenreId = 2, Name = "Non-Fiction" },
            new Genre { GenreId = 3, Name = "Science Fiction" },
            new Genre { GenreId = 4, Name = "Mystery" },
            new Genre { GenreId = 5, Name = "Thriller" },
            new Genre { GenreId = 6, Name = "Romance" },
            new Genre { GenreId = 7, Name = "Fantasy" },
            new Genre { GenreId = 8, Name = "Biography" },
            new Genre { GenreId = 9, Name = "History" }
        );
    }
}