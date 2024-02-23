using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Repositories.AdminRepository.Interfaces;

namespace WebApplication1Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private AdminController _controller;
        private Mock<IAdminRepository> _mockAdminRepository;

        [SetUp]
        public void Setup()
        {
            _mockAdminRepository = new Mock<IAdminRepository>();
            _controller = new AdminController(_mockAdminRepository.Object);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName));
        }

        [Test]
        public void AllUsers_ReturnsViewResultWithUsers()
        {
            // Arrange
            var users = new List<CustomUser> {
                new CustomUser { Id = "1", UserName = "user1", Email = "user1@example.com" },
                new CustomUser { Id = "2", UserName = "user2", Email = "user2@example.com" },
                new CustomUser { Id = "3", UserName = "user3", Email = "user3@example.com" } 
            };
            _mockAdminRepository.Setup(repo => repo.getAllUsers()).Returns(users);

            // Act
            var result = _controller.AllUsers() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<CustomUser>>(result.Model);
        }

        [Test]
        public async Task DeleteUser_WithValidId_DeletesUserAndRedirectsToAllUsers()
        {
            // Arrange
            string userId = "123";
            _mockAdminRepository.Setup(repo => repo.DeleteUser(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(userId) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AllUsers", result.ActionName);
        }
        [Test]
        public void AllTickets_ReturnsViewResultWithTickets()
        {
            // Arrange
            var tickets = new List<Ticket> {
                new Ticket { Id = 1, Title = "Ticket 1", Description = "Description 1", status = "To Do", AssignedTo = "User1", sprint = 1 },
                new Ticket { Id = 2, Title = "Ticket 2", Description = "Description 2", status = "In Process", AssignedTo = "User2", sprint = 2 },
                new Ticket { Id = 3, Title = "Ticket 3", Description = "Description 3", status = "In Review", AssignedTo = "User3", sprint = 1 }
            };
            _mockAdminRepository.Setup(repo => repo.GetAllTickets()).Returns(tickets);

            // Act
            var result = _controller.AllTickets() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<Ticket>>(result.Model);
        }

        [Test]
        public async Task UpdateUser_Post_ValidModel_UpdatesUserAndRedirectsToAllUsers()
        {
            // Arrange
            var user = new CustomUser { Id = "1", UserName = "user1", Email = "user1@example.com", FullName="User 1" };
            _controller.ModelState.Clear();
            _mockAdminRepository.Setup(repo => repo.UpdateUser(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(user) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AllUsers", result.ActionName);
        }

        [Test]
        public async Task UpdateTicket_Post_ValidModel_UpdatesTicketAndRedirectsToAllTickets()
        {
            // Arrange
            var model = new TicketUsersViewModel
            {
                ticket = new Ticket { Id = 2, Title = "Ticket 2", Description = "Description 2", status = "In Process", AssignedTo = "User2", sprint = 2 },
                user = new List<CustomUser> {
                    new CustomUser { Id = "1", UserName = "user1", Email = "user1@example.com" },
                    new CustomUser { Id = "2", UserName = "user2", Email = "user2@example.com" },
                    new CustomUser { Id = "3", UserName = "user3", Email = "user3@example.com" }
                }
            };
            _controller.ModelState.Clear();
            _mockAdminRepository.Setup(repo => repo.UpdateTicket(model)).Verifiable();

            // Act
            var result = _controller.UpdateTicket(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AllTickets", result.ActionName);
            _mockAdminRepository.Verify(repo => repo.UpdateTicket(model), Times.Once);
        }

        [Test]
        public void DeleteTicket_WithValidId_DeletesTicketAndRedirectsToAllTickets()
        {
            // Arrange
            int ticketId = 123;
            _mockAdminRepository.Setup(repo => repo.DeleteTicket(ticketId)).Verifiable();

            // Act
            var result = _controller.DeleteTicket(ticketId) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AllTickets", result.ActionName);
            _mockAdminRepository.Verify(repo => repo.DeleteTicket(ticketId), Times.Once);
        }

    }

}
