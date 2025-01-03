namespace Serwis_Książkowy.Models;

public class Book
{
    public int BookId { get; set; }
    public string Isbn { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    public DateTime PublicationDate { get; set; }
    public float Rating { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();
}