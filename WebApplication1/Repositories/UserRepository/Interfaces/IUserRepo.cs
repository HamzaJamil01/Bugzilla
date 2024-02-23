using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Repositories.UserRepository.Interfaces
{
    public interface IUserRepo
    {
        void CreateTicket(Ticket ticket);
        List<Ticket> GetAllTickets();
        void UpdateTicket(Ticket updatedTicket);
        List<Ticket> GetDevTickets(string name);
        Ticket GetDetails(int id);
        List<string> getAllUsers();
        List<Ticket> GetTicketsAssignedToUserBySprint(string currentUser, int sprint);
        List<int> GetUniqueSprints();
        List<Ticket> GetFilteredTickets(string userFullName, int sprint);
        void UpdateTicketStatus(StatusUpdate updatedTicket);
    }
}
