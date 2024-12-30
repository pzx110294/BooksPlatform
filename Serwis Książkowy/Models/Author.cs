namespace Serwis_Książkowy.Models;

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public ICollection<FavouriteAuthor> FavouriteAuthors { get; set; } = new List<FavouriteAuthor>();
}