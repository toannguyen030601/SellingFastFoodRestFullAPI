using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nhà cung cấp không được vượt quá 100 ký tự")]
        public string SupplierName { get; set; }

        public string ContactName { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
