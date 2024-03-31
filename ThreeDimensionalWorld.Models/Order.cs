using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Потребител")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Адрес")]
        public required int AddressId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Сесия")]
        public required string SessionId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Дата на поръчка")]
        public DateTime DateOrdered { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Дата на пристигане")]
        public DateTime DateOfReceiving { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Цена за доставка")]
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal PriceForDelivery { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Цена")]
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Price { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Получена ли е")]
        public bool IsReceived { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
