using CarWorkshop.Areas.Identity.Data;
using CarWorkshop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(AuthDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var nonAdminUsers = new List<User>();

            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return View(nonAdminUsers);
        }
    

        [HttpPost]
        public IActionResult AddUser()
        {
            Console.WriteLine("Debug message");
            return RedirectToAction("Register", "~/Identity/Pages/Account");
        }



        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    return RedirectToAction("Index");
                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _context.TimeSlot.RemoveRange(_context.TimeSlot.Where(ts => ts.EmployeeID == userId));
                    _context.Ticket.Where(t => t.EmployeeID == userId).ToList().ForEach(t => t.EmployeeID = "");
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            
            return RedirectToAction("Index");
        }
    }

}
