using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.UserRepository.Interfaces;
using authorization = Microsoft.AspNetCore.Authorization;
namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        IUserRepo _userRepo;
        private UserManager<CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUserRepo userRepo, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [authorization.Authorize(Roles = "Manager")]
        public IActionResult Index()
        {
            var viewModel = new TicketViewModel
            {
                Tickets = _userRepo.GetAllTickets(),
                RegisteredUsers = _userRepo.getAllUsers(),
                UniqueSprints = _userRepo.GetUniqueSprints()
            };
            return View(viewModel);
        }
        
        [authorization.Authorize(Roles = "Manager,SQA")]
        [HttpGet]
        public async Task<IActionResult> CreateTicket()
        {
            var allUsers = _userManager.Users.ToList();
            ViewData["users"] = allUsers;
            return View();
        }
        [HttpPost]
        [authorization.Authorize(Roles = "Manager,SQA")]
        public IActionResult CreateTicket(Ticket ticket)
        {
            if(ModelState.IsValid)
            {
                _userRepo.CreateTicket(ticket);
                if (User.IsInRole("Manager"))
                    return RedirectToAction("Index", "User");
                else
                    return RedirectToAction("QAIndex");
            }
            return View();
        }
        [HttpGet]
        [authorization.Authorize(Roles = "Developer")]
        public async Task<IActionResult> DevIndex()
        {
            var tickets = _userRepo.GetAllTickets();
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            var allUsers = _userRepo.getAllUsers();
            List<int> Sprints = _userRepo.GetUniqueSprints();
            myViewModel TicketsModel = new myViewModel { Ticketlist = tickets, CurrentUser = currentUser, RegisteredUsers = allUsers , UniqueSprints = Sprints };
            return View(TicketsModel);
        }
        [HttpGet]
        [authorization.Authorize(Roles = "Developer")]
        public async Task<IActionResult> DevTickets()
        {
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            var viewModel = new TicketViewModel
            {
                Tickets = _userRepo.GetDevTickets(currentUser.FullName),
                UniqueSprints = _userRepo.GetUniqueSprints()
            };
            
            return View(viewModel);
        }
        public async Task<IActionResult> FilteredDevTickets()
        {
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            List<Ticket> Tickets = _userRepo.GetDevTickets(currentUser.FullName);
            myViewModel TicketsModel = new myViewModel { Ticketlist = Tickets, CurrentUser = currentUser };
            return PartialView("_DevPR", TicketsModel);
        }
        [HttpGet]
        [authorization.Authorize(Roles = "SQA")]
        public IActionResult QAIndex()
        {
            var viewModel = new TicketViewModel
            {
                Tickets = _userRepo.GetAllTickets(),
                RegisteredUsers = _userRepo.getAllUsers(),
                UniqueSprints = _userRepo.GetUniqueSprints()
            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult UpdateTicket(int id)
        {
            var ticket = _userRepo.GetDetails(id);
            List<CustomUser> users = _userManager.Users.ToList();
            TicketUsersViewModel model = new TicketUsersViewModel { ticket =  ticket, user = users };
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateTicket(TicketUsersViewModel s) {
            _userRepo.UpdateTicket(s.ticket);
            if (User.IsInRole("SQA"))
                return RedirectToAction("QAIndex");
            else
                return RedirectToAction("DevIndex");
        }
        [HttpPost]
        public IActionResult Updatestatus(StatusUpdate s)
        {
            _userRepo.UpdateTicketStatus(s);
            if (User.IsInRole("SQA"))
                return RedirectToAction("QAIndex");
            else
                return RedirectToAction("DevIndex");
        }
        public IActionResult DevUpdate(int id)
        {
            ViewBag.TicketId = id;
            return View();
        }
        public IActionResult Showticket(int id)
        {
            var data = _userRepo.GetDetails(id);
            return View(data);
        }
        public IActionResult ShowTicketWithUpdate(int id)
        {
            var data = _userRepo.GetDetails(id);
            return View(data);
        }
        public IActionResult ShowDevTickets(int id)
        {
            var data = _userRepo.GetDetails(id);
            return View(data);
        }
        public async Task<IActionResult> FilterSprintTickets(int sprint)
        {
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            var filteredTickets = _userRepo.GetTicketsAssignedToUserBySprint(currentUser.FullName, sprint);
            return PartialView("_TicketsPartialView", filteredTickets);
        }
        public IActionResult FilterTickets(string userFullName, int sprint)
        {
            var filteredTickets = _userRepo.GetFilteredTickets(userFullName, sprint);
            return PartialView("_TicketsPartialView", filteredTickets);
        }
        public IActionResult GetTickets()
        {
            var filteredTickets = _userRepo.GetAllTickets();
            return PartialView("_TicketsPartialView", filteredTickets);
        }
        public IActionResult FilterTicketsWithUpdate(string userFullName, int sprint)
        {
            var filteredTickets = _userRepo.GetFilteredTickets(userFullName, sprint);
            return PartialView("_FilterWithUpdate", filteredTickets);
        }
        public IActionResult GetTicketsWithUpdate()
        {
            var filteredTickets = _userRepo.GetAllTickets();
            return PartialView("_FilterWithUpdate", filteredTickets);
        }
        public async Task<IActionResult> DevFilterTickets(string userFullName, int sprint)
        {
            var filteredTickets = _userRepo.GetFilteredTickets(userFullName, sprint);
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            myViewModel TicketsModel = new myViewModel { Ticketlist = filteredTickets, CurrentUser = currentUser};
            return PartialView("_DevPR", TicketsModel);
        }
        public async Task<IActionResult> GetDevTickets()
        {
            var filteredTickets = _userRepo.GetAllTickets();
            CustomUser currentUser = await _userManager.GetUserAsync(User);
            myViewModel TicketsModel = new myViewModel { Ticketlist = filteredTickets, CurrentUser = currentUser };
            return PartialView("_DevPR", TicketsModel);
        }
    }
}