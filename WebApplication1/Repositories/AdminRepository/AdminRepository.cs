using Microsoft.AspNetCore.Identity;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.AdminRepository.Interfaces;

namespace WebApplication1.Repositories.AdminRepository
{
    public class AdminRepository : IAdminRepository
    {
        private AppDbContext _appDbContext;
        private UserManager<CustomUser> _userManager;

        public AdminRepository(AppDbContext appDbContext, UserManager<CustomUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        public List<Ticket> GetAllTickets()
        {
            return _appDbContext.Ticket.ToList();
        }
        public List<CustomUser> getAllUsers()
        {
            var allUsers = _userManager.Users.ToList();
            return allUsers;
        }
        public CustomUser getUser(string id)
        {
            var user = _appDbContext.Users.Find(id);
            if (user == null)
            {
                return new CustomUser();
            }
            return user;
        }
        public async Task UpdateUser(CustomUser user)
        {
            var userToUpdate = await _userManager.FindByIdAsync(user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.FullName = user.FullName;
                userToUpdate.UserName = user.UserName;
                userToUpdate.Email = user.Email;
                userToUpdate.Designation = user.Designation;
                var rolesToRemove = await _userManager.GetRolesAsync(userToUpdate);
                foreach (var role in rolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(userToUpdate, role);
                }
                var roleToAdd = user.Designation;
                await _userManager.AddToRoleAsync(userToUpdate, roleToAdd);
                await _userManager.UpdateAsync(userToUpdate);
            }
        }
        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var tickets = _appDbContext.Ticket.Where(u => u.AssignedTo == user.FullName);
            foreach (var ticket in tickets)
            {
                ticket.AssignedTo = "Not Assigned.";
            }
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                   await _userManager.RemoveFromRoleAsync(user, role);
                }
                await _userManager.DeleteAsync(user);
            }
            _appDbContext.SaveChanges();
        }
        public Ticket getTicket(int id)
        {
            var ticket = _appDbContext.Ticket.Find(id);
            if (ticket == null)
            {
                return new Ticket();
            }
            return ticket;
        }
        public async Task UpdateTicket(TicketUsersViewModel model)
        {
            var ticket = await _appDbContext.Ticket.FindAsync(model.ticket.Id);
            if(ticket != null)
            {
                ticket.Title = model.ticket.Title;
                ticket.Description = model.ticket.Description;
                ticket.status = model.ticket.status;
                ticket.AssignedTo = model.ticket.AssignedTo;
                ticket.sprint = model.ticket.sprint;
                _appDbContext.SaveChanges();
            }
        }
        public async Task DeleteTicket(int id)
        {
            var ticket =  _appDbContext.Ticket.Find(id);
            if (ticket != null)
            {
                _appDbContext.Ticket.Remove(ticket);
                _appDbContext.SaveChanges();
            }    
        }
    }
}
