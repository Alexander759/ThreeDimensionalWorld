using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Linq.Expressions;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Areas.Public.Models;

namespace ThreeDimensionalWorldWeb.Areas.Public.Controllers
{
    [Area("Public")]
    public class ProductsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 10;

        private static string[] allowedImageFormats =
        [
            ".png",
            ".jpg",
            ".gif"
        ];

        private static string[] allowed3dFormats =
        {
            ".stl",
        };

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(int? page, string? search, int? categoryId)
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
                    string searchTerm = search.ToLower(); // Convert search term to lowercase for case-insensitive comparison
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
                    string searchTerm = search.ToLower(); // Convert search term to lowercase for case-insensitive comparison
                    predicate = p => p.IsActive &&
                                     p.Title.ToLower().Contains(searchTerm);
                }
            }


            // Get total count of products to calculate total pages
            int totalCount = _unitOfWork.ProductRepository.GetAll(predicate).Count();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            // Set the page number, default to 1 if not specified or invalid
            int pageNumber = page ?? 1;
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageNumber = pageNumber > totalPages ? totalPages : pageNumber;

            // Get products for the specified page
            List<Product> products = _unitOfWork.ProductRepository
                .GetAll(predicate, "Files,Category")
                .Skip((pageNumber - 1) * PageSize) // Skip products on previous pages
                .Take(PageSize) // Take products for the current page
                .ToList();

            // Pass additional data to the view if needed (e.g., current page number, total pages, etc.)

            if (!string.IsNullOrEmpty(search))
            {
                ViewData["Search"] = search;
            }

            if (categoryId != null)
            {
                ViewData["CategoryId"] = categoryId;
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["DefaultMaterial"] = _unitOfWork.MaterialRepository.Get(p => true, "Colors");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            return View(products);
        }

        public IActionResult Details(int? id)
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
            

            ViewData["DefaultPrice"] = String.Format("{0:C}", (materials.First().PriceIncrease / 100m + 1) * product.BasePrice);
            ViewData["DefaultColor"] = materials.First().Colors.First().ColorCode;
            ViewData["Materials"] = materials;
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];
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
