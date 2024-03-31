using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;

namespace ThreeDimensionalWorldWeb.Areas.Public.Controllers
{
    [Area("Public")]
    public class CategoriesController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.CategoryRepository.GetAll());
        }
    }
}
