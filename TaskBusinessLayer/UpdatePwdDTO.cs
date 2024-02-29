using System.ComponentModel.DataAnnotations;
namespace TaskBusinessLayer
{
    public class UpdatePwdDTO
    {
       
        public string Email { get; set; }  
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
