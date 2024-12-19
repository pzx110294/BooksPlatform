using Microsoft.EntityFrameworkCore;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data.Configurations;

public static class ReviewConfig
{
    public static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);
    }
}
