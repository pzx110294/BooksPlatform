namespace Serwis_Książkowy.ViewModels;

public class AuthorBookSearchResultsView
{
    public IQueryable<AuthorViewModel>? AuthorViewModels { get; set; }
    public IQueryable<BookViewModel>? BookViewModels { get; set; }
}