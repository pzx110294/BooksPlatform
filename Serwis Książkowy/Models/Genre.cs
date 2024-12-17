namespace Serwis_Książkowy.Models;

public class Genre
{
    public int GenreId { get; set; }
    public string Name { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}