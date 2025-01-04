using System.ComponentModel.DataAnnotations;

namespace Serwis_Książkowy.Models;

public class Review
{
    public int ReviewId { get; set; }
    [Required(ErrorMessage = "Genre name is required")]
    [StringLength(50, ErrorMessage = "Genre name cannot be longer than 50 characters")]
    [Display(Name = "Genre Name")]
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    
    [Required(ErrorMessage = "Book ID is required")]
    [Display(Name = "Book ID")]
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    [Required(ErrorMessage = "Rating is required")]
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    [Display(Name = "Rating")]
    public float Rating { get; set; }
    
    [Required(ErrorMessage = "Review text is required")]
    [StringLength(1000, ErrorMessage = "Review text cannot be longer than 1000 characters")]
    [Display(Name = "Review Text")]
    public string ReviewText { get; set; }
    
    [Required(ErrorMessage = "Creation date is required")]
    [Display(Name = "Created At")]
    [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy}")]
    public DateTime CreatedAt { get; set; }
}