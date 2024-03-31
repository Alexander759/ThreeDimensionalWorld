using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ThreeDimensionalWorld.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Първо име")]
        [PersonalData]
        public string? FirstName { get; set; }

        //[Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Фамилно име")]
        [PersonalData]
        public string? LastName { get; set; }

        [NotMapped]
        public string FullName { get => $"{FirstName} {LastName}"; }

    }
}
