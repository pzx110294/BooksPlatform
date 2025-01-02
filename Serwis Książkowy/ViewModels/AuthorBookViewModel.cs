namespace Serwis_Książkowy.ViewModels;

public class AuthorBookViewModel
{
    public AuthorViewModel AuthorViewModel { get; set; }
    public IEnumerable<BookViewModel> BookViewModel { get; set; }
}