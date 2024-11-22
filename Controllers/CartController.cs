using Microsoft.AspNetCore.Mvc;
using e_BookPvt.Models;
using Newtonsoft.Json;

namespace e_BookPvt.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext _context;

        public CartController(DBContext context)
        {
            _context = context;
        }

        // GET: View Cart
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        // POST: Add to Cart
        [HttpPost]
        public IActionResult AddToCart(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            var cart = GetCartFromSession();
            cart.Add(book);
            SaveCartToSession(cart);

            return RedirectToAction("Index");
        }

        // POST: Remove from Cart
        [HttpPost]
        public IActionResult RemoveFromCart(int bookId)
        {
            var cart = GetCartFromSession();
            var bookToRemove = cart.FirstOrDefault(b => b.BookId == bookId);
            if (bookToRemove != null)
            {
                cart.Remove(bookToRemove);
                SaveCartToSession(cart);
            }

            return RedirectToAction("Index");
        }

        // POST: Clear Cart
        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        // GET: Checkout
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString("CustomerEmail") == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        // POST: Confirm Order
        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            if (HttpContext.Session.GetString("CustomerEmail") == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            var customerEmail = HttpContext.Session.GetString("CustomerEmail");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);

            if (customer == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            foreach (var book in cart)
            {
                var order = new Orders
                {
                    CustomerID = customer.Id,
                    BookID = book.BookId,
                    Quantity = 1,
                    TotalAmount = book.Price,
                    Status = "Processing",
                    Orderdate = DateTime.Now
                };
                _context.Orders.Add(order);
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("CustomerDashboard", "Customers");
        }

        // Helper methods for session management
        private List<Books> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson) ? new List<Books>() : JsonConvert.DeserializeObject<List<Books>>(cartJson);
        }

        private void SaveCartToSession(List<Books> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }
}
