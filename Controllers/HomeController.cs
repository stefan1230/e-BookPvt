using e_BookPvt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace e_BookPvt.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchQuery)
        {
            // Fetch all books or filter by search query
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                books = books.Where(b =>
                    b.Name.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Description.Contains(searchQuery));
                ViewData["SearchQuery"] = searchQuery;
            }

            // Pass session information to the view
            ViewData["CustomerEmail"] = HttpContext.Session.GetString("CustomerEmail");

            return View(books.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
