using System.ComponentModel.DataAnnotations;

namespace Serwis_Książkowy.Models;

public class Author
{
    public int AuthorId { get; set; }
    [StringLength(150, ErrorMessage = "Author name cannot be longer than 200 characters")]
    public string Name { get; set; }
    public ICollection<FavouriteAuthor> FavouriteAuthors { get; set; } = new List<FavouriteAuthor>();
}