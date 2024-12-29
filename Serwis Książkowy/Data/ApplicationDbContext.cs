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
    }
}