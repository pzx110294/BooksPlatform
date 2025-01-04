using System.ComponentModel.DataAnnotations;

namespace Serwis_Książkowy.Models;

public class Review
{
    public int ReviewId { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public float Rating { get; set; }
    public string ReviewText { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy}")]
    public DateTime CreatedAt { get; set; }
}