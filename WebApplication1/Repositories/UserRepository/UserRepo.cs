using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.UserRepository.Interfaces;

namespace WebApplication1.Repositories.UserRepository
{
    public class UserRepo : IUserRepo
    {
        private AppDbContext _appDbContext;
        private UserManager<CustomUser> _userManager;

        public UserRepo(AppDbContext appDbContext, UserManager<CustomUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public void CreateTicket(Ticket ticket)
        {
            _appDbContext.Ticket.Add(ticket);
            _appDbContext.SaveChanges();
        }
        public List<Ticket> GetAllTickets()
        {
            return _appDbContext.Ticket.ToList();
        }
        public void UpdateTicket(Ticket updatedTicket)
        {

            var data = _appDbContext.Ticket.Find(updatedTicket.Id);
            if (data != null)
            {
                data.status = updatedTicket.status;
                data.AssignedTo = updatedTicket.AssignedTo;
            }
            _appDbContext.SaveChanges();
        }
        public List<Ticket> GetDevTickets(string name)
        {
            List<Ticket> tickets = _appDbContext.Ticket
            .Where(t => t.AssignedTo == name)
            .ToList();
            return tickets;
        }
        public Ticket GetDetails(int id)
        {
            var data = _appDbContext.Ticket.Find(id);
            if (data != null)
            {
                return data;
            }
            return new Ticket();
        }
        public List<string> getAllUsers()
        {
            var allUsers = _userManager.Users.ToList();
            List<string> usersFullName = new List<string>();
            foreach (var users in allUsers)
            {
                usersFullName.Add(users.FullName);
            }
            return usersFullName;
        }
        public List<Ticket> GetTicketsAssignedToUserBySprint(string currentUser, int sprint)
        {
            var allTickets = GetDevTickets(currentUser);
            var filteredTickets = allTickets.Where(ticket => ticket.sprint == sprint).ToList();
            return filteredTickets;
        }
        public List<int> GetUniqueSprints()
        {

            var uniqueSprints = _appDbContext.Ticket.Select(ticket => ticket.sprint).Distinct().ToList();
            return uniqueSprints;
        }
        public List<Ticket> GetFilteredTickets(string userFullName, int sprint)
        {
            var filteredTickets = _appDbContext.Ticket.ToList();

            if (!string.IsNullOrEmpty(userFullName))
            {
                filteredTickets = filteredTickets.Where(ticket => ticket.AssignedTo == userFullName).ToList();
            }

            if (sprint > 0)
            {
                filteredTickets = filteredTickets.Where(ticket => ticket.sprint == sprint).ToList();
            }

            return filteredTickets;
        }
        public void UpdateTicketStatus(StatusUpdate updatedTicket)
        {

            var data = _appDbContext.Ticket.Find(updatedTicket.id);
            if (data != null)
            {
                data.status = updatedTicket.status;
            }
            _appDbContext.SaveChanges();
        }

    }
}
