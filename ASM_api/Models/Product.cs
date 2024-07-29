using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string ProductName { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        public decimal Price { get; set; }

        public int CategoryID { get; set; }
        public string Availability { get; set; }
        public int SupplierID { get; set; }
        public string Hinhanh { get; set; }

        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
