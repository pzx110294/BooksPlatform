using System.ComponentModel.DataAnnotations;

namespace Serwis_Książkowy.Models;

public class Genre
{
    public int GenreId { get; set; }
    [Required(ErrorMessage = "Genre name is required")]
    [StringLength(50, ErrorMessage = "Genre name cannot be longer than 50 characters")]
    [Display(Name = "Genre Name")]
    public string Name { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}