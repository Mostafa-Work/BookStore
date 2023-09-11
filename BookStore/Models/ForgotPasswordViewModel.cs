using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ForgotPasswordViewModel
    {
        [Required,EmailAddress,Display(Name ="Registered Email Address")]
        public string Email { get; set; }

        public bool EmailSent { get; set; }
    }
}
