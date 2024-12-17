namespace Serwis_Książkowy.Models;

public class FavouriteAuthor
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}