using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data.Configurations;

public static class FavouriteAuthorConfig
{
    public static void ConfigureFavouriteAuthor(ModelBuilder modelBuilder)
    {
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
    }
}