using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.AccountRepository;
using WebApplication1.Repositories.AccountRepository.Interfaces;
using WebApplication1.Repositories.AdminRepository;
using WebApplication1.Repositories.AdminRepository.Interfaces;
using WebApplication1.Repositories.UserRepository;
using WebApplication1.Repositories.UserRepository.Interfaces;
using YourNamespace.Routing;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
string conStrig = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conStrig));
builder.Services.Configure<IdentityOptions>(options => {
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    }); //For Cutom Authorization or set identity options.
builder.Services.AddIdentity<CustomUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
var app = builder.Build();
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
string[] roleNames = { "Admin", "Manager", "Developer", "SQA" };
foreach (var roleName in roleNames)
{
    var roleExists = await roleManager.RoleExistsAsync(roleName);
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
var adminUser = await userManager.FindByNameAsync("admin");
if (adminUser == null)
{
    adminUser = new CustomUser
    {
        UserName = "admin",
        FullName = "admin",
        Email = "admin@domain.com",
        Designation = "Admin",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        LockoutEnabled = false
    };
    var result = await userManager.CreateAsync(adminUser, "Adminpassword@123");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();
MyRoutingConfig.ConfigureRoutes(app);
app.MapControllers();
app.Run();
