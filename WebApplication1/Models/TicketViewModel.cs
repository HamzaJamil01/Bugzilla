namespace WebApplication1.Models
{
    public class TicketViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public List<string> RegisteredUsers { get; set; }
        public string SelectedUserFullName { get; set; }
        public List<int> UniqueSprints { get; set; }
    }
}
