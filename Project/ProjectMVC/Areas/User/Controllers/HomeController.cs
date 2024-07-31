using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ProjectMVC.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _db;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _db = unitOfWork;

        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _db.Product.GetAll(includeProperties: "Category");
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _db.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    shoppingCart.ApplicationUserId = userId;

                    ShoppingCart cartDB = _db.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
                    if (cartDB != null)
                    {
                        cartDB.Count += shoppingCart.Count;
                        _db.ShoppingCart.Update(cartDB);
                    }
                    else
                    {
                        _db.ShoppingCart.Add(shoppingCart);
                    }
                    _db.Save();
                    return RedirectToAction("Index");
                }
            }

            // Handle the case where the user ID couldn't be retrieved
            TempData["ErrorMessage"] = "Unable to process your request. Please try again.";
            return RedirectToAction("Error");
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
