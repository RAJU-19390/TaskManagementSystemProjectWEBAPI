using System.ComponentModel.DataAnnotations;
namespace TaskManagementWebAPI.Models
{
    public class UpdatePwdModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public string OldPassword { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,10}$",
           ErrorMessage = "Password must be between 6 and 10 characters long and include at least one letter and one digit.")]
        public string NewPassword { get; set; }

    }
}