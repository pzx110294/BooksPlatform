using System.ComponentModel.DataAnnotations;

namespace Serwis_Książkowy.Models;

public class Book
{
    public int BookId { get; set; }
    [Required(ErrorMessage = "ISBN is required")]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "ISBN has to be 13 characters long")]
    [Display(Name = "ISBN")]
    public string Isbn { get; set; }
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Author is required")]
    [Display(Name = "Author")]
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    [Required(ErrorMessage = "Genre is required")]
    [Display(Name = "Genre")]
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy}")]
    [Display(Name = "Publication Date")]
    public DateTime PublicationDate { get; set; }
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    [Display(Name = "Rating")]
    public float? Rating { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();
}