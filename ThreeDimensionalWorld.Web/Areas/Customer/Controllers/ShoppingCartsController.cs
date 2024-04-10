using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using ThreeDimensionalWorld.DataAccess.Repository;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Web.Areas.Customer.Models;
using ThreeDimensionalWorld.Web.OrderPriceConfiguration;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;

namespace ThreeDimensionalWorld.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.CustomerRole)]
    public class ShoppingCartsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ShippingPrice _shippingPrice;

        public ShoppingCartsController(IUnitOfWork unitOfWork, ShippingPrice shippingPrice)
        {
            _unitOfWork = unitOfWork;
            _shippingPrice = shippingPrice;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId, "ShoppingCartItems");
            
            if(shoppingCart == null)
            {
                return NotFound();
            }

            List<ShoppingCartItem> shoppingCartItems = _unitOfWork
                .ShoppingCartItemRepository
                .GetAll(s => shoppingCart.ShoppingCartItems.Select(sh => sh.Id)
                    .ToList()
                    .Contains(s.Id), 
                    "Material,Color,ShoppingCart")
                .ToList();

            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                shoppingCartItems[i].Product = _unitOfWork
                    .ProductRepository
                    .Get(p => p.Id == shoppingCartItems[i].ProductId, "Files");
            }

            decimal shipping = _shippingPrice.Price;
            ViewData["Shipping"] = shipping;
            ViewData["Total"] = shoppingCartItems.Select(s => s.GetPrice()).Sum() + shipping;

            return View(shoppingCartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(ShoppingCartItem shoppingCartItem)
        {
            if (ModelState.IsValid)
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if(string.IsNullOrEmpty(userId))
                {
                    return NotFound();
                }

                ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(a => a.Id == userId);

                if(applicationUser == null)
                {
                    return NotFound();
                }

                ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId);

                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart() { UserId = userId };
                    _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                    _unitOfWork.Save();
                }

                ShoppingCartItem? shoppingCartItemAdded = _unitOfWork.ShoppingCartItemRepository
                    .Get(s => s.MaterialId == shoppingCartItem.MaterialId
                && s.ColorId == shoppingCartItem.ColorId
                && s.ProductId == shoppingCartItem.ProductId
                && s.ShoppingCartId == shoppingCart.Id);
                
                if(shoppingCartItemAdded != null)
                {
                    shoppingCartItemAdded.Quantity += shoppingCartItem.Quantity;
                    _unitOfWork.ShoppingCartItemRepository.Update(shoppingCartItemAdded);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }

                shoppingCartItem.ShoppingCartId = shoppingCart.Id;
                _unitOfWork.ShoppingCartItemRepository.Add(shoppingCartItem);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        [Route("api/[controller]/AddToCart")]
        [HttpPost]
        public IActionResult AddToCartApi([FromBody] ShoppingCartItemCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound(new { message = "User id not found!" });
                }

                ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(a => a.Id == userId);

                if (applicationUser == null)
                {
                    return NotFound(new { message = "User not found!" });
                }

                ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId);


                if (shoppingCart == null)
                {
                    return NotFound(new { message = "Shopping cart not found!" });
                }

                ShoppingCartItem? shoppingCartItemAdded = _unitOfWork.ShoppingCartItemRepository
                    .Get(s => s.MaterialId == dto.MaterialId
                && s.ColorId == dto.ColorId
                && s.ProductId == dto.ProductId
                && s.ShoppingCartId == shoppingCart.Id);

                if (shoppingCartItemAdded != null)
                {
                    shoppingCartItemAdded.Quantity += dto.Quantity;
                    _unitOfWork.ShoppingCartItemRepository.Update(shoppingCartItemAdded);
                    _unitOfWork.Save();
                    return Ok(new { message = "Success!"});
                }

                ShoppingCartItem shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = shoppingCart.Id,
                    ColorId = dto.ColorId,
                    ProductId = dto.ProductId,
                    MaterialId = dto.MaterialId,
                    Quantity = dto.Quantity
                };

                _unitOfWork.ShoppingCartItemRepository.Add(shoppingCartItem);
                _unitOfWork.Save();
                return Ok(new { message = "Success!" });
            }

            return BadRequest(new {error = "Model state was invalid"});
        }

        [Route("api/[controller]/RemoveFromCart")]
        [HttpPost]
        public IActionResult RemoveFromCart([FromBody] int id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(new { message = "User id not found!" });
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(a => a.Id == userId);

            if (applicationUser == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId, "ShoppingCartItems");
            
            if (shoppingCart == null)
            {
                return NotFound(new { message = "Shopping cart not found!" });
            }

            ShoppingCartItem? shoppingCartItem = shoppingCart.ShoppingCartItems.Where(s => s.Id == id).FirstOrDefault();

            if (shoppingCartItem == null)
            {
                return NotFound( new { message = "Shopping cart item not found" });
            }

            _unitOfWork.ShoppingCartItemRepository.Remove(shoppingCartItem);
            _unitOfWork.Save();

            List<ShoppingCartItem> shoppingCartItems = _unitOfWork.ShoppingCartItemRepository.GetAll(s => s.ShoppingCartId == shoppingCart.Id,
                "Product,Material").ToList();

            decimal newTotal = shoppingCartItems.Select(s => s.GetPrice()).Sum() + _shippingPrice.Price;

            return Ok(new { message = "Success", newTotal = String.Format("{0:C}", newTotal), hasNoElements = shoppingCart.ShoppingCartItems.Count() == 0, shippingPrice = String.Format("{0:C}", _shippingPrice.Price) });
        }

        [HttpPost]
        [Route("api/[controller]/ShoppingCartItemChangeQuantity")]
        public IActionResult ShoppingCartItemChangeQuantity([FromBody] ShoppingCartItemChangeQuantityDto dataToBeSent)
        {
            if(dataToBeSent.NewQuantity <= 0)
            {
                return BadRequest(new { message = "Quantity can't be less than or equal to 0!" });
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(new { message = "User id not found!" });
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(a => a.Id == userId);

            if (applicationUser == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            ShoppingCart? shoppingCart = _unitOfWork.ShoppingCartRepository.Get(s => s.UserId == userId, "ShoppingCartItems");

            if (shoppingCart == null)
            {
                return NotFound(new { message = "Shopping cart not found!" });
            }

            ShoppingCartItem? shoppingCartItem = _unitOfWork
                .ShoppingCartItemRepository
                .Get(s => s.ShoppingCartId == shoppingCart.Id && s.Id == dataToBeSent.Id,
                    "Product,Material");

            if (shoppingCartItem == null)
            {
                return NotFound(new { message = "Shopping cart item not found" });
            }


            shoppingCartItem.Quantity = dataToBeSent.NewQuantity;
            _unitOfWork.ShoppingCartRepository.Update(shoppingCart);
            _unitOfWork.Save();

            List<ShoppingCartItem> shoppingCartItems = _unitOfWork.ShoppingCartItemRepository.GetAll(s => s.ShoppingCartId == shoppingCart.Id,
                "Product,Material").ToList();

            decimal newTotal = shoppingCartItems.Select(s => s.GetPrice()).Sum() + _shippingPrice.Price;


            return Ok(new { message = "Success", priceItem = String.Format("{0:C}", shoppingCartItem.GetPrice()), newTotal = String.Format("{0:C}", newTotal), shippingPrice = String.Format("{0:C}", _shippingPrice.Price) });
        }
    }
}
