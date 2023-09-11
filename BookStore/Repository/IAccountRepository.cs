using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Repository
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(SignUpViewModel viewModel);
        Task<SignInResult> PasswordSignInAsync(LoginViewModel viewModel);
        Task SignOutAsync();
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel viewModel);

        //For Email Confirmation
        Task<IdentityResult> ConfirmEmailAsync(string uid,string token);
        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        //For Password Forgot
        Task GenerateForgetPasswordTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel viewModel);

    }
}
