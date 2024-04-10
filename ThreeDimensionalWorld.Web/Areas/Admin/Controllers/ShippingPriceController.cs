using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.Web.Areas.Admin.Models;
using ThreeDimensionalWorld.Web.OrderPriceConfiguration;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.AdminRole)]
    public class ShippingPriceController : Controller
    {
        private readonly ShippingPrice _shippingPrice;

        public ShippingPriceController(ShippingPrice shippingPrice)
        {
            _shippingPrice = shippingPrice;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            ShippingPriceVM orderPriceVM = new ShippingPriceVM { Price = _shippingPrice.Price };
            return View(orderPriceVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShippingPriceVM orderPriceVM)
        {
            if (ModelState.IsValid)
            {
                _shippingPrice.Price = orderPriceVM.Price;
                return RedirectToAction(nameof(Edit));
            }

            return View(orderPriceVM);
        }
    }
}
