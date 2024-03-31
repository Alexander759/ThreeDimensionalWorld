using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Security.Policy;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Configuration;

namespace ThreeDimensionalWorldWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppConfiguration.CustomerRole)]
    public class OrdersController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            IEnumerable<Order> orders = _unitOfWork.OrderRepository.GetAll(o => o.UserId == userId, "Address");
            return View(orders);
        }

        [HttpGet]
        public IActionResult Confirm()
        {
            if (TempData["Session"] == null)
            {
                return NotFound();
            }

            var service = new SessionService();
            Session session = service.Get(TempData["Session"]!.ToString());

            if (session == null || session.PaymentStatus.ToLower() != "paid")
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            if (applicationUser == null)
            {
                return NotFound();
            }

            ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId, "ShoppingCartItems");

            if (shoppingCart == null)
            {
                return NotFound();
            }

            List<ShoppingCartItem> shoppingCartItems = _unitOfWork
                .ShoppingCartItemRepository
                .GetAll(s => shoppingCart.ShoppingCartItems.Select(sh => sh.Id)
                    .ToList()
                    .Contains(s.Id),
                    "Material,Product")
                .ToList();


            Order order = new Order()
            {
                UserId = userId,
                DateOrdered = DateTime.Now,
                DateOfReceiving = DateTime.Now.AddDays(3),
                PriceForDelivery = 0,
                SessionId = session.Id,
                AddressId = (int)TempData["AddressId"]!,
                Price = shoppingCartItems.Select(s => s.GetPrice()).Sum()
            };

            _unitOfWork.OrderRepository.Add(order);
            _unitOfWork.Save();

            List<OrderItem> orderItems = new List<OrderItem>();

            
            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                var shoppingCartItem = shoppingCartItems[i];

                
                OrderItem orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    ColorId = shoppingCartItem.ColorId,
                    MaterialId = shoppingCartItem.MaterialId,
                    Quantity = shoppingCartItem.Quantity,
                    ProductId = shoppingCartItem.ProductId,
                    PricePerUnit = shoppingCartItem.GetPricePerUnit(),
                    TotalPrice = shoppingCartItem.GetPrice()
                };
                _unitOfWork.ShoppingCartItemRepository.Remove(shoppingCartItem);
                _unitOfWork.Save();

                _unitOfWork.OrderItemRepository.Add(orderItem);
                _unitOfWork.Save();
                
            }
            

            return RedirectToAction("Details", new { id=order.Id});
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Address address)
        {
            if (ModelState.IsValid)
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound();
                }

                ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

                if (applicationUser == null)
                {
                    return NotFound();
                }

                ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId, "ShoppingCartItems");

                if (shoppingCart == null)
                {
                    return NotFound();
                }


                Uri uri = new Uri(Request.GetDisplayUrl());

                string domain = uri.Authority;

                var options = new SessionCreateOptions
                {
                    SuccessUrl = $"https://{domain}/Customer/Orders/Confirm",
                    CancelUrl = $"https://{domain}/Customer/ShoppingCarts",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode="payment"
                };

                List<ShoppingCartItem> shoppingCartItems = _unitOfWork
                .ShoppingCartItemRepository
                .GetAll(s => shoppingCart.ShoppingCartItems.Select(sh => sh.Id)
                    .ToList()
                    .Contains(s.Id),
                    "Material,Product")
                .ToList();

                if(shoppingCartItems.Count == 0)
                {
                    return NotFound();
                }

                foreach(var item in shoppingCartItems)
                {
                    var sessionItemList = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.GetPricePerUnit() * 100),
                            Currency = "bgn",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product?.Title
                            }
                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionItemList);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.AddressRepository.Add(address);
                _unitOfWork.Save();

                TempData["Session"] = session.Id;
                TempData["AddressId"] = address.Id;

                Response.Headers.Append("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return View(address);
        }

        public IActionResult Details(int? id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            Order? order = _unitOfWork.OrderRepository.Get(o => o.Id == id, "Address");

            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }

            List<OrderItem> orderItems = _unitOfWork.OrderItemRepository.GetAll(o => o.OrderId == order.Id, "Material,Color").ToList();

            for (int i = 0; i < orderItems.Count(); i++)
            {
                orderItems[i].Product = _unitOfWork.ProductRepository.Get(p => p.Id == orderItems[i].ProductId, "Files");
            }

            order.OrderItems = orderItems;

            return View(order);
        }
    }
}

