using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories.AdminRepository;
using WebApplication1.Repositories.AdminRepository.Interfaces;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        List<CustomUser> Users { get; set; }
        IAdminRepository _adminRepository;
        public AdminController(IAdminRepository adminRepository) 
        {
            _adminRepository = adminRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllUsers()
        {
            Users = _adminRepository.getAllUsers();
            return View(Users);
        }
        public IActionResult AllTickets()
        {
            List<Ticket> Tickets = _adminRepository.GetAllTickets();
            return View(Tickets);
        }
        [HttpGet]
        public IActionResult UpdateUser(string id)
        {
            CustomUser user = _adminRepository.getUser(id);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(CustomUser user)
        {
            if(ModelState.IsValid)
            {
                await _adminRepository.UpdateUser(user);
                return RedirectToAction("AllUsers");
            }
            ModelState.AddModelError(string.Empty, "Failed to update user.");
            return View();
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _adminRepository.DeleteUser(id);
            return RedirectToAction("AllUsers");
        }
        [HttpGet]
        public IActionResult UpdateTicket(int id)
        {
            Ticket ticket = _adminRepository.getTicket(id);
            Users = _adminRepository.getAllUsers();
            TicketUsersViewModel model = new TicketUsersViewModel { ticket = ticket , user = Users };
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateTicket(TicketUsersViewModel model)
        {
            _adminRepository.UpdateTicket(model);
            return RedirectToAction("AllTickets");
        }
        public IActionResult DeleteTicket(int id)
        {
            _adminRepository.DeleteTicket(id);
            return RedirectToAction("AllTickets");
        }
    }
}
