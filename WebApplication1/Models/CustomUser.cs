using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class CustomUser : IdentityUser
    {
            public string FullName { get; set; }
            public string Designation { get; set; }
    }
}
