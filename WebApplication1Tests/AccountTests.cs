using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Repositories.AccountRepository.Interfaces;

namespace WebApplication1.Account.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _controller;
        private Mock<IAccountRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IAccountRepository>();
            _controller = new AccountController(_mockRepository.Object);
        }

        // SignUp action tests
        [Test]
        public async Task SignUp_Post_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var manager = new UserModel();

            // Setup mock repository to return success
            _mockRepository.Setup(repo => repo.SignUp(It.IsAny<UserModel>()))
                           .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.SignUp(manager) as RedirectToActionResult;

            // Debugging statements
            Console.WriteLine("Controller action executed.");
            Console.WriteLine($"Result type: {result?.GetType().Name}");

            // Assert
            Assert.IsNotNull(result); // Ensure the result is not null
            Assert.AreEqual("Login", result.ActionName); // Ensure it redirects to "Index" action
            Assert.AreEqual("Account", result.ControllerName); // Ensure it redirects to "User" controller
        }
        [Test]
        public async Task SignUp_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var manager = new UserModel();
            _controller.ModelState.AddModelError("Email", "Email is required.");

            // Act
            var result = await _controller.SignUp(manager) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.Count > 0);
            Assert.IsNull(result.ViewName); // Ensure it returns the same view
        }
        // VerifyEmail action tests

        [Test]
        public async Task VerifyEmail_ValidInput_ReturnsRedirectToActionResult()
        {
            // Arrange
            const string email = "test@example.com";
            const string token = "test_token";
            _mockRepository.Setup(repo => repo.VerifyEmail(email, token)).ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyEmail(email, token) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("User", result.ControllerName);
        }

        [Test]
        public async Task VerifyEmail_InvalidInput_ReturnsBadRequestResult()
        {
            // Arrange
            const string email = null;
            const string token = null;

            // Act
            var result = await _controller.VerifyEmail(email, token) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Email Or Token.", result.Value);
        }
        [Test]
        public async Task Login_Post_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var manager = new UserModel { UserName = "testuser", Password = "testpassword" };
            var signInResult = Microsoft.AspNetCore.Identity.SignInResult.Success;
            _mockRepository.Setup(repo => repo.Login(manager.UserName, manager.Password))
                           .ReturnsAsync(signInResult);

            // Act
            var result = await _controller.Login(manager) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("RoleCheck", result.ActionName);
        }

        [Test]
        public async Task SignOut_ReturnsRedirectToActionResult()
        {

            // Act
            var result = await _controller.SignOut() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("User", result.ControllerName);
        }
        [Test]
        public async Task ForgetPassword_Post_ValidEmail_ReturnsViewResult()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = await _controller.ForgetPassword(email) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName); // Ensure it returns the same view
        }


        [Test]
        public async Task ForgetPassword_Post_InvalidEmail_ReturnsViewResultWithModelError()
        {
            // Arrange
            var email = "";

            // Act
            var result = await _controller.ForgetPassword(email) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName); // Ensure it returns the correct view
        }

        // ResetPassword action tests

        [Test]
        public async Task ResetPassword_Post_ValidData_ReturnsContentResult()
        {
            // Arrange
            var fp = new ForgetPassword { Email = "test@example.com", Token = "test_token" };
            // Assuming ResetPassword method accepts a ForgetPassword parameter
            _mockRepository.Setup(repo => repo.ResetPassword(It.IsAny<ForgetPassword>()))
                           .ReturnsAsync(IdentityResult.Success);


            // Act
            var result = await _controller.ResetPassword(fp) as ContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Password has been updated.", result.Content);
        }

        [Test]
        public async Task ResetPassword_Post_InvalidData_ReturnsBadRequestResult()
        {
            // Arrange
            var fp = new ForgetPassword { Email = "test@example.com", Token = "test_token" };
            _mockRepository.Setup(repo => repo.ResetPassword(fp)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _controller.ResetPassword(fp) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error Occured.", result.Value);
        }
    }
}
