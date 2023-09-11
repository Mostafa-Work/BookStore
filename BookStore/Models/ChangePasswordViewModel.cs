using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ChangePasswordViewModel
    {
        [Required,Display(Name="Current Password"),DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [Required, Display(Name = "New Password"), DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, Display(Name = "Confirm New Password"), DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Passwords Dosen't Match")]
        public string ConfirmNewPassword { get; set; }
    }
}
