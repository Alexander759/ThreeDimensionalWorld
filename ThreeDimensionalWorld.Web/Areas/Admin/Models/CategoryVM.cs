using System.ComponentModel.DataAnnotations;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Models
{
    public class CategoryVM
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Име")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Описание")]
        public required string Description { get; set; }

        [Display(Name = "Снимка")]
        public IFormFile? Image { get; set; }
    }
}
