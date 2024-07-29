using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        public decimal Price { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
