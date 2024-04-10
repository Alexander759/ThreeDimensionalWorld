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
        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Първо име")]
        [PersonalData]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Фамилно име")]
        [PersonalData]
        public required string LastName { get; set; }

        [Display(Name = "Пълно име")]
        [NotMapped]
        public string FullName { get => $"{FirstName} {LastName}"; }

    }
}
