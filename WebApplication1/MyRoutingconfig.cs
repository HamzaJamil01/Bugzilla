using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace YourNamespace.Routing
{
    public static class MyRoutingConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
               name: "default",
               pattern: "{controller=User}/{action=Index}/{id?}",
               defaults: true);
            endpoints.MapControllerRoute(
                name: "user",
                pattern: "{controller=User}/{action=Index}/{id?}");
           
        }
    }
}
