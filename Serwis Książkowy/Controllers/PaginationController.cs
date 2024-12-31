using Microsoft.AspNetCore.Mvc;

namespace Serwis_Książkowy.Controllers
{
    public class PaginationController : Controller
    {
        protected void SetPaginationData(int currentPage, int totalPages)
        {
            int previousPage = currentPage <= 1 ? 1 : currentPage - 1; 
            int nextPage = currentPage >= totalPages ? totalPages : currentPage + 1;

            ViewData["CurrentPage"] = currentPage;
            ViewData["PreviousPage"] = previousPage;
            ViewData["NextPage"] = nextPage;
            ViewData["TotalPages"] = totalPages;
        }
    }
}
