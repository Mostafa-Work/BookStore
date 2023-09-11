using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountRepository accountRepository;
        public AccountController(IAccountRepository accountRepository) 
        {
            this.accountRepository = accountRepository;
        }
        [AllowAnonymous]
        public IActionResult GetUserInfo()
        {
            List<string> userClaims = new List<string>();
            foreach(var claim in User.Claims)
                userClaims.Add(claim.Value);

            return Json(userClaims);
        }
        [AllowAnonymous]
        public IActionResult SignUp ()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await accountRepository.CreateUserAsync(viewModel);
                if (result.Succeeded == true)
                {
                    ModelState.Clear();
                    return RedirectToAction("ConfirmEmail", new {email = viewModel.Email });
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await accountRepository.PasswordSignInAsync(viewModel);
                if (result.Succeeded == true)
                    return RedirectToAction("AllBooks", "Book");

                else if (result.IsNotAllowed)
                    return RedirectToAction("ConfirmEmail", new { email = viewModel.Email });

                else if (result.IsLockedOut)
                    ModelState.AddModelError("", "Account blocked. Try after some time.");
                else
                    ModelState.AddModelError("", "Invalid Credientials");
            }
            return View(viewModel);
        }

        public async Task<IActionResult> LogOut()
        {
            await accountRepository.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var result =await accountRepository.ChangePasswordAsync(viewModel);
                if (result.Succeeded == true)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View(viewModel);
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
             }
            return View(viewModel);
        }

        [AllowAnonymous]
        public  async Task<IActionResult> ConfirmEmail(string uid,string token,string email)
        {
            EmailConfirmationViewModel viewModel = new EmailConfirmationViewModel
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result =await accountRepository.ConfirmEmailAsync(uid, token);
                if(result.Succeeded == true)
                {
                    viewModel.EmailVerified = true;
                }
            }

            return View(viewModel);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmationViewModel viewModel)
        {
            var user = await accountRepository.GetUserByEmailAsync(viewModel.Email);
            if (user != null)
            {
                if (user.EmailConfirmed == true)
                {
                    viewModel.EmailVerified = true;
                    return View(viewModel);
                }
                await accountRepository.GenerateEmailConfirmationTokenAsync(user);
                viewModel.EmailSent = true;
                ModelState.Clear();
            }
            else
                ModelState.AddModelError("", "Something went wrong");

            return View(viewModel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user =await accountRepository.GetUserByEmailAsync(viewModel.Email);
                if(user != null)
                    await accountRepository.GenerateForgetPasswordTokenAsync(user);
                ModelState.Clear();
                viewModel.EmailSent= true;
            }
            return View(viewModel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string uid,string token)
        {
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel
            {
                UserId= uid,
                Token= token
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword (ResetPasswordViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                viewModel.Token = viewModel.Token.Replace(' ', '+');
                var result =await accountRepository.ResetPasswordAsync(viewModel);
                if(result.Succeeded)
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    return View(viewModel);
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(viewModel);
        }

    }
}
