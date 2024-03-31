using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThreeDimensionalWorld.DataAccess.Repository;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Areas.Customer.Models;
using ThreeDimensionalWorldWeb.Configuration;

namespace ThreeDimensionalWorldWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppConfiguration.CustomerRole)]
    public class ShoppingCartsController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public ShoppingCartsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

            return View(shoppingCartItems);
        }

        [HttpPost]
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

            return Ok(new {message="Success"});
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

            return Ok(new { message = "Success", priceItem = String.Format("{0:C}", shoppingCartItem.GetPrice()) });
        }
    }
}
