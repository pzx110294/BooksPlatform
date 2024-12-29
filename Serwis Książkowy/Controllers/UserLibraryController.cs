using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serwis_Książkowy.Data;
using Serwis_Książkowy.Models;
using Serwis_Książkowy.Models.Enums;

namespace Serwis_Książkowy.Controllers
{
    public class UserLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: UserLibrary
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToLibrary(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userId)) {return NotFound();}

            var userLibrary = new UserLibrary
            {
                UserId = userId,
                BookId = bookId,
                Status = Status.Completed,
                AddedAt = DateTime.Now
            };
            _context.UserLibraries.Add(userLibrary);
            _context.SaveChanges();
            return RedirectToAction("Index", "Books");
            
        }

    }
}
