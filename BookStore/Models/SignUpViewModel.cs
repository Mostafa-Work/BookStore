using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="Please Enter Your First Name")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage ="Please Enter Last Name")]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Please Enter Your Email")]
        [EmailAddress(ErrorMessage ="Please Enter a Valid Email Address")]
        [Display(Name ="Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter a password")]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords does not match")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Password Confirmation")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
