using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        public string CategoryName { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
