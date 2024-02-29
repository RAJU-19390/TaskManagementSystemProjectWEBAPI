using System.ComponentModel.DataAnnotations;

namespace TaskManagementWebAPI.Models
{
    public class UserInfoModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,10}$",
            ErrorMessage = "Password must be between 6 and 10 characters long and include at least one letter and one digit.")]
        public string Password { get; set; }

        public bool Is_Admin { get; set; }
    }
}
