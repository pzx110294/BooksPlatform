using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data.Configurations;

public static class UserLibraryConfig
{
    public static void ConfigureUserLibrary(ModelBuilder modelBuilder)
    {
        
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
    }
}