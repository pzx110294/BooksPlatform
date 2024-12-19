using Microsoft.AspNetCore.Identity;

namespace Serwis_Książkowy.Models;

public class AppUser : IdentityUser
{
    public ICollection<UserLibrary>? UserLibraries { get; set; } = new List<UserLibrary>();
    public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    public ICollection<FavouriteAuthor>? FavouriteAuthors { get; set; } = new List<FavouriteAuthor>();
}