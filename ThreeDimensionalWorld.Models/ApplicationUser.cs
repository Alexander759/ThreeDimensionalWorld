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
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int? CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        public void InitializeCart()
        {
            Cart = new Cart { UserId = this.Id };
        }

    }
}
