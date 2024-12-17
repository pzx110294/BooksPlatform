using Serwis_Książkowy.Models.Enums;
namespace Serwis_Książkowy.Models;

public class UserLibrary
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public Status Status { get; set; }
    public DateTime AddedAt { get; set; }
}

