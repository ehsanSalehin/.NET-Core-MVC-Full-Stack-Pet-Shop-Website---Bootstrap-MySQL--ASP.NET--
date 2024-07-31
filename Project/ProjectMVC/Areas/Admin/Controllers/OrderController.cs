using Microsoft.AspNetCore.Mvc;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Utilities;
using System.Collections.Generic;
using System.Linq;
using Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stripe;

namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty] public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDeatils.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            if (OrderVM.OrderHeader == null)
            {
                return NotFound();
            }

            return View(OrderVM);
        }


        [HttpPost]
        [Authorize(Roles = Details1.Role_Admin)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Updated Successfully";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = Details1.Role_Admin)]
        public IActionResult CancelOrder(){

         var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader.PaymentStatus == Details1.PaymentStatusApprpved) {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntetId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Details1.StatusCancelled, Details1.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Details1.StatusCancelled, Details1.StatusCancelled);
            }
            _unitOfWork.Save();

            TempData["success"] = "Cancelled Successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }



        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeader;

            if (User.IsInRole(Details1.Role_Admin))
            {
                objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        var userId = userIdClaim.Value;
                        objOrderHeader = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }

            switch (status)
            {
                case "pending":
                    objOrderHeader = objOrderHeader.Where(u => u.PaymentStatus == Details1.PaymentStatusPending || u.PaymentStatus == Details1.PaymentStatusDelay);
                    break;
                case "inprocess":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == Details1.StatusInProcess);
                    break;
                case "approved":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == Details1.StatusApprpved);
                    break;
                case "completed":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == Details1.StatusShipped);
                    break;
                default:
                    break;
            }

            return Json(new { data = objOrderHeader });
        }
    }
    }