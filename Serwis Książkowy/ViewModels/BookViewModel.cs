using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;

namespace Serwis_Książkowy.ViewModels;

public class BookViewModel
{
    public Book Book { get; set; }
    public bool IsInLibrary { get; set; }
    public Status? Status { get; set; }
}