using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Linq.Expressions;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Web.Areas.Public.Models;

namespace ThreeDimensionalWorld.Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class ProductsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, int? materialId, int? colorId)
        {
            Expression<Func<Product, bool>> predicate;

            if (categoryId != null)
            {
                if (string.IsNullOrEmpty(search))
                {
                    predicate = p => p.IsActive && p.CategoryId == categoryId;
                }
                else
                {
                    string searchTerm = search.ToLower();
                    predicate = p => p.IsActive && p.CategoryId == categoryId &&
                                     p.Title.ToLower().Contains(searchTerm) ;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(search))
                {
                    predicate = p => p.IsActive;
                }
                else
                {
                    string searchTerm = search.ToLower();
                    predicate = p => p.IsActive &&
                                     p.Title.ToLower().Contains(searchTerm);
                }
            }

            List<Product> products = _unitOfWork.ProductRepository
                .GetAll(predicate, "Files,Category")
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                ViewData["Search"] = search;
            }

            if (categoryId != null)
            {
                ViewData["CategoryId"] = categoryId;
            }


            Material defaultMaterial = null!;
            MaterialColor defaultMaterialColor = null!;

            if(materialId == null || materialId == 0)
            {
                defaultMaterial = _unitOfWork.MaterialRepository.GetAll("Colors").FirstOrDefault()!;

                if (colorId == null || colorId == 0)
                {
                    defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                }
                else
                {
                    defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault(c => c.Id == colorId)!;

                    if (defaultMaterialColor == null)
                    {
                        defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                    }
                }

            }
            else
            {
                defaultMaterial= _unitOfWork.MaterialRepository.Get(m => m.Id == materialId, "Colors")!;

                if (defaultMaterial == null)
                {
                    defaultMaterial = _unitOfWork.MaterialRepository.GetAll("Colors").FirstOrDefault()!;


                    if (colorId == null || colorId == 0)
                    {
                        defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                    }
                    else
                    {
                        defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault(c => c.Id == colorId)!;

                        if (defaultMaterialColor == null)
                        {
                            defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                        }
                    }
                }
                else
                {

                    if (colorId == null || colorId == 0)
                    {
                        defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                    }
                    else
                    {
                        defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault(c => c.Id == colorId)!;

                        if (defaultMaterialColor == null)
                        {
                            defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault()!;
                        }
                    }
                }
            }


            Dictionary<Product, decimal> prices = new Dictionary<Product, decimal>();

            foreach(var item in products)
            {
                decimal price = item.BasePrice * (1 + defaultMaterial.PriceIncrease / 100m);
                prices.Add(item, price);
            }

            if(minPrice != null && maxPrice != null)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    if (prices[products[i]] < minPrice)
                    {
                        prices.Remove(products[i]);
                        products.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (prices[products[i]] > maxPrice)
                    {
                        prices.Remove(products[i]);
                        products.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (minPrice != null)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    if (prices[products[i]] < minPrice)
                    {
                        prices.Remove(products[i]);
                        products.RemoveAt(i);
                        i--;
                    }
                    
                }
            }
            else if (maxPrice != null)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    if (prices[products[i]] > maxPrice)
                    {
                        prices.Remove(products[i]);
                        products.RemoveAt(i);
                        i--;
                    }
                }
            }


            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            ViewData["Materials"] = _unitOfWork.MaterialRepository.GetAll("Colors").ToList();
            ViewData["Prices"] = prices;
            ViewData["DefaultMaterial"] = defaultMaterial;
            ViewData["DefaultMaterialColor"] = defaultMaterialColor;
            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int? id, int? materialId, int colorId)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id, "Files,Category");

            if(product == null || !product.IsActive)
            {
                return NotFound();
            }

            List<Material> materials = _unitOfWork.MaterialRepository.GetAll("Colors").ToList();

            Material? defaultMaterial = materials.FirstOrDefault(m => m.Id == materialId); 

            if (defaultMaterial == null)
            {
                defaultMaterial = materials.First();
            }

            MaterialColor? defaultMaterialColor = defaultMaterial.Colors.FirstOrDefault(c => c.Id == colorId);

            if (defaultMaterialColor == null)
            {
                defaultMaterialColor = defaultMaterial.Colors.First();
            }

            ViewData["DefaultPrice"] = String.Format("{0:C}", (defaultMaterial.PriceIncrease / 100m + 1) * product.BasePrice);
            ViewData["DefaultColor"] = defaultMaterialColor.ColorCode;
            ViewData["Materials"] = materials;
            ViewData["ModelProduct"] = product;
            return View();
        }

        [HttpPost]
        [Route("api/[controller]/GetProductPrice")]
        public IActionResult GetProductPrice([FromBody] GetProductPriceDTO dto)
        {
            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId);
            
            if(dto.Quantity <= 0)
            {
                return BadRequest("Quantity can't be less than or equal to 0");
            }

            if(product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            Material? material = _unitOfWork.MaterialRepository.Get(m => m.Id == dto.MaterialId);

            if(material == null)
            {
                return NotFound(new { message = "Material not found" });
            }

            return Ok(new {price = String.Format("{0:C}", (material.PriceIncrease / 100m + 1m) * product.BasePrice * dto.Quantity), message="Success"});
        }
    }
}
