using Microsoft.EntityFrameworkCore;

namespace Serwis_Książkowy.Data.Configurations;

public static class ModelConfig
{
    public static void ConfigureModel(this ModelBuilder modelBuilder)
    {
        UserLibraryConfig.ConfigureUserLibrary(modelBuilder);
        ReviewConfig.ConfigureReview(modelBuilder);
        FavouriteAuthorConfig.ConfigureFavouriteAuthor(modelBuilder);
    }
}