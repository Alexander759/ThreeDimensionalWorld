using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.AdminRole)]
    public class OrdersController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Order> orders = _unitOfWork.OrderRepository.GetAll("Address,ApplicationUser").ToList();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            Order? order = _unitOfWork.OrderRepository.Get(o => o.Id == id, "Address,ApplicationUser");

            if (order == null)
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

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            Order? order = _unitOfWork.OrderRepository.Get(o => o.Id == id, "Address,ApplicationUser");

            if (order == null)
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


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            Order? order = _unitOfWork.OrderRepository.Get(o => o.Id == id, "Address,ApplicationUser,OrderItems");

            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.AddressRepository.Remove(order.Address!);
            _unitOfWork.Save();

            _unitOfWork.OrderItemRepository.RemoveRange(order.OrderItems);
            _unitOfWork.Save();

            _unitOfWork.OrderRepository.Remove(order);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
