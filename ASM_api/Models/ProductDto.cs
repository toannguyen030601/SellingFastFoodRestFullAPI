using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class ProductDto
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public string Availability { get; set; }
        public int SupplierID { get; set; }
        public IFormFile? FileHinhAnh { get; set; }
    }
}
