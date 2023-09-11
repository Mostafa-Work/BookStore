using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="Please Enter Your Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
