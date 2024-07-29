using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        public string Address { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
    }
}
