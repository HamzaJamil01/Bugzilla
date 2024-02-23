using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public string AssignedTo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sprint no must be greater than 0.")]
        public int sprint { get; set; }
    }
}
