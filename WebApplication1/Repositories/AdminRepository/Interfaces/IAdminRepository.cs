using WebApplication1.Models;

namespace WebApplication1.Repositories.AdminRepository.Interfaces
{
    public interface IAdminRepository
    {
        List<Ticket> GetAllTickets();
        List<CustomUser> getAllUsers();
        CustomUser getUser(string id);
        Task UpdateUser(CustomUser user);
        Task DeleteUser(string userId);
        Ticket getTicket(int id);
        Task UpdateTicket(TicketUsersViewModel model);
        Task DeleteTicket(int id);
    }
}
