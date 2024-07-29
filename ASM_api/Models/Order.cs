using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_api.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc")]
        public DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Tổng số tiền là bắt buộc")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
