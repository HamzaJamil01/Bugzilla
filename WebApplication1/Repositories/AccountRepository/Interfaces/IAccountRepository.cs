using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Repositories.AccountRepository.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUp(UserModel manager);
        Task<bool> VerifyEmail(string email, string token);
        Task<SignInResult> Login(string userName, string Password);
        Task SignOut();
        Task ForgetPassword(string email);
        Task<IdentityResult> ResetPassword(ForgetPassword fp);
    }
}
