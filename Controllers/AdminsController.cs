//using e_BookPvt.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace E_Book_System.Controllers
//{
//    public class AdminsController : Controller
//    {
//        private readonly DBContext _context;

//        public AdminsController(DBContext context)
//        {
//            _context = context;
//        }

//        // GET: Admins
//        public async Task<IActionResult> Index()
//        {
//            return View(await _context.Admins.ToListAsync());
//        }

//        // GET: Admins/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var admin = await _context.Admins
//                .FirstOrDefaultAsync(m => m.AdminId == id);
//            if (admin == null)
//            {
//                return NotFound();
//            }

//            return View(admin);
//        }

//        // GET: Admins/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Admins/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("AdminId,Name,Email,Password,NICNo")] Admin admin)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(admin);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(admin);
//        }

//        // GET: Admins/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var admin = await _context.Admins.FindAsync(id);
//            if (admin == null)
//            {
//                return NotFound();
//            }
//            return View(admin);
//        }

//        // POST: Admins/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Name,Email,Password,NICNo")] Admin admin)
//        {
//            if (id != admin.AdminId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(admin);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!AdminExists(admin.AdminId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(admin);
//        }

//        // GET: Admins/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var admin = await _context.Admins
//                .FirstOrDefaultAsync(m => m.AdminId == id);
//            if (admin == null)
//            {
//                return NotFound();
//            }

//            return View(admin);
//        }

//        // POST: Admins/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var admin = await _context.Admins.FindAsync(id);
//            if (admin != null)
//            {
//                _context.Admins.Remove(admin);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool AdminExists(int id)
//        {
//            return _context.Admins.Any(e => e.AdminId == id);
//        }
//    }
//}



using e_BookPvt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace e_BookPvt.Controllers
{
    public class AdminController : Controller
    {
        private readonly DBContext _context;

        public AdminController(DBContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            Console.WriteLine($"Hashed Password: {hashedPassword}");

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email && a.Password == hashedPassword);

            if (admin == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            // Debug Output
            Console.WriteLine($"Admin Found: {admin.Name}");

            HttpContext.Session.SetString("AdminEmail", admin.Email);
            HttpContext.Session.SetString("AdminName", admin.Name);

            return RedirectToAction("Dashboard");
        }


        // GET: Admin/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Admin/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Email,Password,NICNo")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == admin.Email);
                if (existingAdmin != null)
                {
                    ViewBag.ErrorMessage = "Email is already registered.";
                    return View(admin);
                }

                admin.Password = HashPassword(admin.Password);

                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(admin);
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Ensure Admin is logged in
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            ViewData["AdminName"] = HttpContext.Session.GetString("AdminName");
            return View();
        }



        // Other admin management functionalities
        public IActionResult ManageBooks()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var books = _context.Books.ToList();
            return View(books);
        }

        public async Task<IActionResult> ManageCustomers()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        public async Task<IActionResult> ManageOrders()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Book)
                .ToListAsync();

            return View(orders);
        }

        //public IActionResult GenerateReports()
        //{
        //    if (HttpContext.Session.GetString("AdminEmail") == null)
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    // Add reporting logic here
        //    return View();
        //}


        // GET: Admin/CreateBook
        public IActionResult CreateBook()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Admin/CreateBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBook([Bind("Name,Author,Description,Price,ImageUrl")] Books book)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageBooks");
            }
            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageBooks));
        }


        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }


        // GET: Admin/EditBook/5
        public async Task<IActionResult> EditBook(int? id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Admin/EditBook/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(int id, [Bind("BookId,Name,Author,Description,Price,ImageUrl")] Books book)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageBooks));
            }
            return View(book);
        }



        public IActionResult CreateCustomer()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer([Bind("CustomerName,Email,Password,ContactNo,NICNo")] Customers customer)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                customer.Password = HashPassword(customer.Password); // Hash the password
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageCustomers));
            }

            return View(customer);
        }



        public async Task<IActionResult> EditCustomer(int? id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, [Bind("Id,CustomerName,Email,Password,ContactNo,NICNo")] Customers customer)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.Password = HashPassword(customer.Password); // Hash the password
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageCustomers));
            }

            return View(customer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


        public async Task<IActionResult> DeleteCustomer(int? id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }



        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageCustomers));
        }

        //public IActionResult CreateOrder()
        //{
        //    ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName");
        //    ViewBag.Books = new SelectList(_context.Books, "BookId", "Name");
        //    ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price); // For price calculation
        //    return View();
        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateOrder(Orders order)
        //{
        //    // Logging for debugging
        //    Console.WriteLine($"CustomerID: {order.CustomerID}, BookID: {order.BookID}, Quantity: {order.Quantity}, TotalAmount: {order.TotalAmount}");

        //    if (!ModelState.IsValid)
        //    {
        //        // Log validation errors
        //        foreach (var error in ModelState)
        //        {
        //            Console.WriteLine($"Key: {error.Key}, Error: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
        //        }

        //        // Repopulate dropdowns if validation fails
        //        ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName");
        //        ViewBag.Books = new SelectList(_context.Books, "BookId", "Name");
        //        ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

        //        return View(order);
        //    }

        //    // Fallback calculation for TotalAmount
        //    if (order.TotalAmount <= 0 || order.TotalAmount == null)
        //    {
        //        var book = _context.Books.FirstOrDefault(b => b.BookId == order.BookID);
        //        if (book != null)
        //        {
        //            order.TotalAmount = order.Quantity * book.Price;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("BookID", "Selected book is invalid.");
        //            ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName");
        //            ViewBag.Books = new SelectList(_context.Books, "BookId", "Name");
        //            ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);
        //            return View(order);
        //        }
        //    }

        //    // Default values for missing fields
        //    order.Status = "Processing";
        //    order.Orderdate = DateTime.Now;

        //    try
        //    {
        //        // Add to database and save changes
        //        _context.Orders.Add(order);
        //        _context.SaveChanges();
        //        Console.WriteLine("Order inserted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error inserting order: {ex.Message}");
        //        ModelState.AddModelError(string.Empty, "An error occurred while saving the order. Please try again.");
        //        ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName");
        //        ViewBag.Books = new SelectList(_context.Books, "BookId", "Name");
        //        ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);
        //        return View(order);
        //    }

        //    return RedirectToAction("ManageOrders");
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateOrder(Orders order)
        //{
        //    Console.WriteLine($"CustomerID: {order.CustomerID}, BookID: {order.BookID}, Quantity: {order.Quantity}, TotalAmount: {order.TotalAmount}");

        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var key in ModelState.Keys)
        //        {
        //            var errors = ModelState[key].Errors.Select(e => e.ErrorMessage).ToList();
        //            Console.WriteLine($"Key: {key}, Errors: {string.Join(", ", errors)}");
        //        }
        //        // Repopulate dropdowns
        //        ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName", order.CustomerID);
        //        ViewBag.Books = new SelectList(_context.Books, "BookId", "Name", order.BookID);
        //        ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

        //        return View(order);
        //    }

        //    // Default values
        //    order.Status = "Processing";
        //    order.Orderdate = DateTime.Now;

        //    // Save to database
        //    _context.Orders.Add(order);
        //    _context.SaveChanges();

        //    return RedirectToAction("ManageOrders");
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateOrder()
        //{
        //    // Retrieve values from the form explicitly
        //    var customerId = Request.Form["CustomerID"];
        //    var bookId = Request.Form["BookID"];
        //    var quantity = Request.Form["Quantity"];
        //    var totalAmount = Request.Form["TotalAmount"];
        //    var status = Request.Form["Status"];
        //    var orderDate = Request.Form["Orderdate"];

        //    Orders order = new Orders();

        //    if (int.TryParse(customerId, out int parsedCustomerId))
        //    {
        //        order.CustomerID = parsedCustomerId;
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("CustomerID", "Customer is required.");
        //    }

        //    if (int.TryParse(bookId, out int parsedBookId))
        //    {
        //        order.BookID = parsedBookId;
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("BookID", "Book is required.");
        //    }

        //    if (int.TryParse(quantity, out int parsedQuantity) && parsedQuantity > 0)
        //    {
        //        order.Quantity = parsedQuantity;
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
        //    }

        //    if (decimal.TryParse(totalAmount, out decimal parsedTotalAmount))
        //    {
        //        order.TotalAmount = parsedTotalAmount;
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("TotalAmount", "Total amount must be valid.");
        //    }

        //    order.Status = !string.IsNullOrWhiteSpace(status) ? status : "Processing";
        //    if (!DateTime.TryParse(orderDate, out DateTime parsedOrderDate))
        //    {
        //        order.Orderdate = DateTime.Now;
        //    }
        //    else
        //    {
        //        order.Orderdate = parsedOrderDate;
        //    }

        //    Console.WriteLine($"CustomerID: {order.CustomerID}, BookID: {order.BookID}, Quantity: {order.Quantity}, TotalAmount: {order.TotalAmount}");

        //    // If there are validation errors, repopulate dropdowns and return the view
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var key in ModelState.Keys)
        //        {
        //            var errors = ModelState[key].Errors.Select(e => e.ErrorMessage).ToList();
        //            Console.WriteLine($"Key: {key}, Errors: {string.Join(", ", errors)}");
        //        }

        //        ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName", order.CustomerID);
        //        ViewBag.Books = new SelectList(_context.Books, "BookId", "Name", order.BookID);
        //        ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

        //        return View(order);
        //    }

        //    // Save order to database
        //    _context.Orders.Add(order);
        //    _context.SaveChanges();

        //    return RedirectToAction("ManageOrders");
        //}


        // GET: CreateOrder
        public IActionResult CreateOrderView()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName");
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Name");
            ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

            return View();
        }

        // POST: CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder()
        {
            // Explicitly parse form values
            var customerId = Request.Form["CustomerID"];
            var bookId = Request.Form["BookID"];
            var quantity = Request.Form["Quantity"];
            var totalAmount = Request.Form["TotalAmount"];
            var status = Request.Form["Status"];
            var orderDate = Request.Form["Orderdate"];

            Orders order = new Orders();

            if (int.TryParse(customerId, out int parsedCustomerId))
            {
                order.CustomerID = parsedCustomerId;
            }
            else
            {
                ModelState.AddModelError("CustomerID", "Customer is required.");
            }

            if (int.TryParse(bookId, out int parsedBookId))
            {
                order.BookID = parsedBookId;
            }
            else
            {
                ModelState.AddModelError("BookID", "Book is required.");
            }

            if (int.TryParse(quantity, out int parsedQuantity) && parsedQuantity > 0)
            {
                order.Quantity = parsedQuantity;
            }
            else
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
            }

            if (decimal.TryParse(totalAmount, out decimal parsedTotalAmount))
            {
                order.TotalAmount = parsedTotalAmount;
            }
            else
            {
                ModelState.AddModelError("TotalAmount", "Total amount must be valid.");
            }

            order.Status = !string.IsNullOrWhiteSpace(status) ? status : "Processing";

            if (!DateTime.TryParse(orderDate, out DateTime parsedOrderDate))
            {
                order.Orderdate = DateTime.Now;
            }
            else
            {
                order.Orderdate = parsedOrderDate;
            }

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName", order.CustomerID);
                ViewBag.Books = new SelectList(_context.Books, "BookId", "Name", order.BookID);
                ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

                return View("CreateOrderView", order);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("ManageOrders");
        }


        [HttpGet]
        public async Task<IActionResult> EditOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Book)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName", order.CustomerID);
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Name", order.BookID);
            ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrder()
        {
            // Explicitly parse form values
            var orderId = Request.Form["OrderId"];
            var customerId = Request.Form["CustomerID"];
            var bookId = Request.Form["BookID"];
            var quantity = Request.Form["Quantity"];
            var totalAmount = Request.Form["TotalAmount"];
            var status = Request.Form["Status"];
            var orderDate = Request.Form["Orderdate"];

            if (!int.TryParse(orderId, out int parsedOrderId))
            {
                return NotFound();
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == parsedOrderId);

            if (order == null)
            {
                return NotFound();
            }

            if (int.TryParse(customerId, out int parsedCustomerId))
            {
                order.CustomerID = parsedCustomerId;
            }
            else
            {
                ModelState.AddModelError("CustomerID", "Customer is required.");
            }

            if (int.TryParse(bookId, out int parsedBookId))
            {
                order.BookID = parsedBookId;
            }
            else
            {
                ModelState.AddModelError("BookID", "Book is required.");
            }

            if (int.TryParse(quantity, out int parsedQuantity))
            {
                order.Quantity = parsedQuantity;
            }
            else
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
            }

            if (decimal.TryParse(totalAmount, out decimal parsedTotalAmount))
            {
                order.TotalAmount = parsedTotalAmount;
            }
            else
            {
                ModelState.AddModelError("TotalAmount", "Total amount must be valid.");
            }

            order.Status = string.IsNullOrEmpty(status) ? "Processing" : status.ToString();


            if (DateTime.TryParse(orderDate, out DateTime parsedOrderDate))
            {
                order.Orderdate = parsedOrderDate;
            }
            else
            {
                order.Orderdate = DateTime.Now;
            }

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                ViewBag.Customers = new SelectList(_context.Customers, "Id", "CustomerName", customerId);
                ViewBag.Books = new SelectList(_context.Books, "BookId", "Name", bookId);
                ViewBag.BookPrices = _context.Books.ToDictionary(b => b.BookId, b => b.Price);

                return View(order);
            }

            // Update the order in the database
            _context.Update(order);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageOrders));
        }








        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }


        public async Task<IActionResult> DeleteOrder(int? id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrderConfirmed(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageOrders));
        }



        // GET: Admin/GenerateReports
        public IActionResult GenerateReports(DateTime? startDate, DateTime? endDate)
        {
            // Build the query
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Book)
                .AsQueryable();

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(o => o.Orderdate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.Orderdate <= endDate.Value);
            }

            // Group and summarize the data
            var reportData = query
                .GroupBy(o => new { o.CustomerID, o.BookID })
                .Select(g => new Report
                {
                    CustomerName = g.FirstOrDefault().Customer.CustomerName,
                    BookName = g.FirstOrDefault().Book.Name,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderDate = g.FirstOrDefault().Orderdate
                })
                .ToList();

            return View(reportData);
        }

        // GET: Admin/ExportReportsToCSV
        public IActionResult ExportReportsToCSV(DateTime? startDate, DateTime? endDate)
        {
            // Build the query
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Book)
                .AsQueryable();

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(o => o.Orderdate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.Orderdate <= endDate.Value);
            }

            // Group and summarize the data
            var reportData = query
                .GroupBy(o => new { o.CustomerID, o.BookID })
                .Select(g => new Report
                {
                    CustomerName = g.FirstOrDefault().Customer.CustomerName,
                    BookName = g.FirstOrDefault().Book.Name,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderDate = g.FirstOrDefault().Orderdate
                })
                .ToList();

            // Create CSV content
            var csv = new StringBuilder();
            csv.AppendLine("CustomerName,BookName,TotalOrders,TotalRevenue,OrderDate");

            foreach (var report in reportData)
            {
                csv.AppendLine($"{report.CustomerName},{report.BookName},{report.TotalOrders},{report.TotalRevenue},{report.OrderDate:yyyy-MM-dd}");
            }

            // Return CSV file
            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "Reports.csv");
        }


        [HttpGet]
        public IActionResult SignOut()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to Login page
            return RedirectToAction("Login", "Admin");
        }

    }
}
