using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Models.ViewModels;
using Project.Utilities;
using Stripe.Checkout;
using System.Security.Claims;
using Stripe;

namespace ProjectMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class Cart : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty] public ShoppingCartVM ShoppingCartVM { get; set; }
        public Cart(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var shoppingCartVM = new ShoppingCartVM
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                        u => u.ApplicationUserId == userId,
                        includeProperties: "Product"
                    ),
                    OrderTotal = 0
                };

                foreach (var cart in shoppingCartVM.ShoppingCartList)
                {
                    cart.price = GetPrice(cart);
                    shoppingCartVM.OrderTotal += (cart.price * cart.Count);
                }

                return View(shoppingCartVM);
            }


            return RedirectToAction("Login", "Account");
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User ID: {userId}, Type: {userId?.GetType().Name}");

            if (userId != null)
            {
                var shoppingCartVM = new ShoppingCartVM
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                        u => u.ApplicationUserId == userId,
                        includeProperties: "Product"
                    ),
                    OrderHeader = new OrderHeader(),
                    OrderTotal = 0
                };

                shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
                Console.WriteLine($"ApplicationUser ID: {shoppingCartVM.OrderHeader.ApplicationUser.Id}, Type: {shoppingCartVM.OrderHeader.ApplicationUser.Id.GetType().Name}");

                shoppingCartVM.OrderHeader.ApplicationUserId = userId;
                shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
                shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
                shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
                shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

                foreach (var cart in shoppingCartVM.ShoppingCartList)
                {
                    cart.price = GetPrice(cart);
                    shoppingCartVM.OrderTotal += (cart.price * cart.Count);
                }

                return View(shoppingCartVM);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Initialize ShoppingCartVM if it's null
            if (ShoppingCartVM == null)
            {
                ShoppingCartVM = new ShoppingCartVM();
            }

            // Initialize OrderHeader if it's null
            if (ShoppingCartVM.OrderHeader == null)
            {
                ShoppingCartVM.OrderHeader = new OrderHeader();
            }

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product"
            );

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = applicationUser.Name ?? "Unknown";
            ShoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber ?? "Unknown";
            ShoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddress ?? "Unknown";
            ShoppingCartVM.OrderHeader.City = applicationUser.City ?? "Unknown";
            ShoppingCartVM.OrderHeader.State = applicationUser.State ?? "Unknown";
            ShoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode ?? "Unknown";

            ShoppingCartVM.OrderTotal = 0;
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.price = GetPrice(cart);
                ShoppingCartVM.OrderTotal += (cart.price * cart.Count);
            }

            ShoppingCartVM.OrderHeader.PaymentStatus = Details1.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = Details1.StatusApprpved;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            //stripes

            StripeConfiguration.ApiKey = "secret";



            var domain = "https://localhost:7004/";
            var checkoutOptions = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"User/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "User/Cart/Index",

            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                checkoutOptions.LineItems.Add(sessionLineItem);
            }

            var checkoutService = new SessionService();

            Session session = checkoutService.Create(checkoutOptions);

            _unitOfWork.OrderHeader.UpdateStripePayment(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDeatils.Add(orderDetail);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Summary), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader == null)
            {
                return NotFound();
            }

            if (orderHeader.PaymentStatus != Details1.PaymentStatusDelay)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePayment(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Details1.StatusApprpved, Details1.PaymentStatusApprpved);
                    _unitOfWork.Save();
                }
            }

            var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else 
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
        }


        private double GetPrice (ShoppingCart shoppingCart) {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            } else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
          }
        }
    }
}
