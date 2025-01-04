using System.ComponentModel.DataAnnotations;
using Serwis_Książkowy.Models.Enums;
namespace Serwis_Książkowy.Models;

public class UserLibrary
{
    [Required(ErrorMessage = "User ID is required")]
    [Display(Name = "User ID")]
    public string UserId { get; set; }
    public AppUser User { get; set; }
    
    
    [Required(ErrorMessage = "Book ID is required")]
    [Display(Name = "Book ID")]
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    [Required(ErrorMessage = "Status is required")]
    [Display(Name = "Status")]
    public Status Status { get; set; }
    
    [Required(ErrorMessage = "Added date is required")]
    [Display(Name = "Added At")]
    [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy}")]
    public DateTime AddedAt { get; set; }
}

