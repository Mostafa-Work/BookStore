using BookStore.Data;
using BookStore.Models;
using BookStore.Service;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IUserService userService,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.userService = userService;
            this.roleManager = roleManager;
            this.emailService= emailService;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync (string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUserAsync(SignUpViewModel viewModel)
        {
            ApplicationUser user = new ApplicationUser()
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                UserName=viewModel.Email
            };
            var result =await  userManager.CreateAsync(user,viewModel.Password);
            if(result.Succeeded)
                await GenerateEmailConfirmationTokenAsync(user);
            return result;
            
        }

        public async Task<SignInResult> PasswordSignInAsync (LoginViewModel viewModel)
        {
            return await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, true);
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel viewModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userService.GetUserId());
            return await userManager.ChangePasswordAsync(user, viewModel.CurrentPassword,viewModel.NewPassword);
        }


        //EmailConfirmation Repository Methods
        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token =await  userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmailConfirmationEmailAsync(user, token);
        }
        private async Task SendEmailConfirmationEmailAsync (ApplicationUser user, string token)
        {
            string appDomain = configuration["Application:AppDomain"];
            string confirmationLink = configuration["Application:EmailConfirmation"];
            UserEmailOptions userEmailOptions = new UserEmailOptions
            {
                ToEmails = new List<string> {user.Email},
                PlaceHolders=new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("{{UserName}}",user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",string.Format(appDomain+confirmationLink,user.Id,token))
                }
            };
            await emailService.SendEmailForEmailConfirmation(userEmailOptions);
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            return await userManager.ConfirmEmailAsync(await userManager.FindByIdAsync(uid), token);
        }


        //Forget Password and Reset Methods Repository
        public async Task GenerateForgetPasswordTokenAsync(ApplicationUser user)
        {
            string token =await userManager.GeneratePasswordResetTokenAsync(user);
            await SendForgotPasswordEmailAsync(user,token);
        }
        private async Task SendForgotPasswordEmailAsync(ApplicationUser user, string token)
        {
            string appDomain = configuration["Application:AppDomain"];
            string confirmationLink = configuration["Application:ForgotPassword"];

            UserEmailOptions userEmailOptions = new UserEmailOptions
            {
                ToEmails=new List<string> { user.Email},
                PlaceHolders=new List<KeyValuePair<string,string>> 
                {
                    new KeyValuePair<string,string>("{{UserName}}",user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",string.Format(appDomain+confirmationLink,user.Id,token))
                }
            };

            await emailService.SendEmailForForgotPassword(userEmailOptions);
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel viewModel)
        {
            return await userManager.ResetPasswordAsync(await userManager.FindByIdAsync(viewModel.UserId), viewModel.Token, viewModel.NewPassword);
        }
    }
}
