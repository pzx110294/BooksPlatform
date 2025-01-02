namespace Serwis_Książkowy.ViewModels;

public class AuthorBookViewModel
{
    public AuthorViewModel AuthorViewModel { get; set; }
    public IQueryable<BookViewModel> BookViewModel { get; set; }
}