using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Repositories.UserRepository.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApplication1.User.Tests.Controllers
{
    public static class UserManagerHelper
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }

    public static class RoleManagerHelper
    {
        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            return new Mock<RoleManager<TRole>>(store.Object, null, null, null, null);
        }
    }
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<IUserRepo> _mockUserRepo;
        private Mock<UserManager<CustomUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockUserManager = UserManagerHelper.MockUserManager<CustomUser>();
            _mockRoleManager = RoleManagerHelper.MockRoleManager<IdentityRole>();

            _controller = new UserController(_mockUserRepo.Object, _mockUserManager.Object, _mockRoleManager.Object);
        }

        [Test]
        public void Index_WithValidUser_ReturnsViewResult()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetAllTickets()).Returns(new List<Ticket>());
            _mockUserRepo.Setup(repo => repo.getAllUsers()).Returns(new List<string>());
            _mockUserRepo.Setup(repo => repo.GetUniqueSprints()).Returns(new List<int>());

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task CreateTicket_Get_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = await _controller.CreateTicket() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task DevIndex_Get_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = await _controller.DevIndex() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task UpdateTicket_Get_ReturnsViewResult()
        {
            // Arrange
            int ticketId = 1;

            // Act
            var result =  _controller.UpdateTicket(ticketId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task DevTickets_Get_ReturnsViewResult()
        {
            // Arrange
            var user = new CustomUser { FullName = "TestUser" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.DevTickets() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task FilteredDevTickets_Get_ReturnsPartialViewResult()
        {
            // Arrange
            var user = new CustomUser { FullName = "TestUser" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.FilteredDevTickets() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PartialViewResult>(result);
        }
    }


}
