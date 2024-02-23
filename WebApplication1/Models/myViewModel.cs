namespace WebApplication1.Models
{
    public class myViewModel
    {
        public List<Ticket> Ticketlist { get; set; }
        public CustomUser CurrentUser { get; set; }
        public List<string> RegisteredUsers { get; set; }
        public string SelectedUserFullName { get; set; }
        public List<int> UniqueSprints { get; set; }
    }
}
