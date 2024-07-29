using System.ComponentModel.DataAnnotations;

namespace ASM_api.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public string Password { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vị trí là bắt buộc")]
        public string Position { get; set; }
    }
}
