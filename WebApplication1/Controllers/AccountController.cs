using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories.AccountRepository.Interfaces;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel manager)
        {
            if(ModelState.IsValid)
            {
              var result = await _accountRepository.SignUp(manager);
                if(result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult SignUp() {

            return View();
        }
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid Email Or Token.");
            }

            var result = await _accountRepository.VerifyEmail(email, token);

            if (result)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return BadRequest("An error occured.");

            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserModel manager)
        {
            var login = await _accountRepository.Login(manager.UserName, manager.Password);

            if (login.IsNotAllowed)
            {
                ModelState.AddModelError("", "Confirm email before login.");
                return View();
            }
            if (!login.Succeeded) {
                ModelState.AddModelError("", "UserName or Password is wrong.");
                return View();
            }
            return RedirectToAction("RoleCheck");

        }
        public async Task<IActionResult> SignOut()
        {
            await _accountRepository.SignOut();
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            await _accountRepository.ForgetPassword(email);
            return View();
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        public IActionResult GetEmail(string email, string token)
        {

            var fp = new ForgetPassword { Email = email, Token = token };
            return View(fp);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgetPassword fp)
        {
            var result = await _accountRepository.ResetPassword(fp);
            if(result.Succeeded)
            {
                return Content("Password has been updated.");
            }
            else
            {
                return BadRequest("Error Occured.");
            }

        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult RoleCheck()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Manager"))
                    return RedirectToAction("Index", "User");
                if (User.IsInRole("Developer"))
                    return RedirectToAction("DevIndex", "User");
                if (User.IsInRole("SQA"))
                    return RedirectToAction("QAIndex", "User");
            }
            return RedirectToAction("AccessDenied");
        }
    }
}
