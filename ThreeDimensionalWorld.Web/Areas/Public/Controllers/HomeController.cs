using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Web.Models;

namespace ThreeDimensionalWorld.Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Categories"] = _unitOfWork.CategoryRepository.GetAll().ToList().Take(3).ToList();

            ViewData["Products"] = _unitOfWork.ProductRepository
                .GetAll("Files,Category")
                .Take(3)
                .ToList();

            ViewData["DefaultMaterial"] = _unitOfWork.MaterialRepository.GetAll("Colors").FirstOrDefault();

            return View();
        }


        [HttpGet]
        public IActionResult Contacts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AboutUs()
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
