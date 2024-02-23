using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Net;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Repositories.AccountRepository.Interfaces;

namespace WebApplication1.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        UserManager<CustomUser> _userManager;
        SignInManager<CustomUser> _signInManager;

        public AccountRepository(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> SignUp(UserModel manager)
        {
            var customUser = new CustomUser { UserName = manager.UserName, Email = manager.Email, FullName = manager.Name, Designation = manager.Designation };

            var result = await _userManager.CreateAsync(customUser, manager.Password);
            if (result.Succeeded)
            {
                var role = string.Empty;
                switch (manager.Designation)
                {
                    case "Developer":
                        role = "Developer";
                        break;
                    case "Manager":
                        role = "Manager";
                        break;
                    case "SQA":
                        role = "SQA";
                        break;
                }
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(customUser, role);
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(customUser);
                var url = "https://bugzillawebapp.azurewebsites.net";
                var verificationUrl = $"{url}/Account/VerifyEmail?email={Uri.EscapeDataString(manager.Email)}&token={Uri.EscapeDataString(token)}";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Bugzilla", "imhamzajamil@gmail.com"));
                message.To.Add(new MailboxAddress(manager.Name, manager.Email));
                message.Subject = "Verify Your Email Address";
                message.Body = new TextPart("plain")
                {
                    Text = "Click the following link for verification : " + verificationUrl
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.elasticemail.com", 2525, false);
                    client.Authenticate("imhamzajamil@gmail.com", "2A962F43019BB454DD8DBF75270A1D6A2122");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            return result;
        }
        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<SignInResult> Login(string userName, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, Password, true, false);
            return result;
        }
        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var url = "https://bugzillawebapp.azurewebsites.net";
                var verificationUrl = $"{url}/Account/GetEmail?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                Console.WriteLine(verificationUrl);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Bugzilla", "imhamzajamil@gmail.com"));
                message.To.Add(new MailboxAddress("User", email));
                message.Subject = "Verify Your Email Address";
                message.Body = new TextPart("plain")
                {
                    Text = "Click the following link to reset your password: " + verificationUrl
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.elasticemail.com", 2525, false);
                    client.Authenticate("imhamzajamil@gmail.com", "2A962F43019BB454DD8DBF75270A1D6A2122");
                    client.Send(message);
                    client.Disconnect(true);
                }

            }


        }
        public async Task<IdentityResult> ResetPassword(ForgetPassword fp)
        {
            var user = await _userManager.FindByEmailAsync(fp.Email);
            return await _userManager.ResetPasswordAsync(user, fp.Token, fp.Password);
        }
    }
}