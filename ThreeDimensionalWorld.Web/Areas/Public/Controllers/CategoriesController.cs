using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;

namespace ThreeDimensionalWorld.Web.Areas.Public.Controllers
{
    [Area("Public")]
    public class CategoriesController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_unitOfWork.CategoryRepository.GetAll());
        }
    }
}
