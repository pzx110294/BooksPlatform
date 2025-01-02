using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.ViewModels;

public class AuthorViewModel
{
    public Author Author { get; set; }
    public bool IsFollowed { get; set; }
}