using CarWorkshop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CarWorkshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using CarWorkshop.Models;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeScheduleEditController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeScheduleEditController(AuthDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);

            WeeklySchedule employeeSchedule = new WeeklySchedule
            {
                Monday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 1 && ts.TicketID == 0).ToList(),
                Tuesday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 2 && ts.TicketID == 0).ToList(),
                Wednesday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 3 && ts.TicketID == 0).ToList(),
                Thursday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 4 && ts.TicketID == 0).ToList(),
                Friday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 5 && ts.TicketID == 0).ToList(),
                Saturday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 6 && ts.TicketID == 0).ToList(),
                Sunday = _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && (int)ts.BeginTime.DayOfWeek == 0 && ts.TicketID == 0).ToList()
            };

            // Check if any of the days' time slots are null and initialize them to empty lists if null
            employeeSchedule.Monday ??= new List<TimeSlot>();
            employeeSchedule.Tuesday ??= new List<TimeSlot>();
            employeeSchedule.Wednesday ??= new List<TimeSlot>();
            employeeSchedule.Thursday ??= new List<TimeSlot>();
            employeeSchedule.Friday ??= new List<TimeSlot>();
            employeeSchedule.Saturday ??= new List<TimeSlot>();
            employeeSchedule.Sunday ??= new List<TimeSlot>();


            return View(employeeSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> AddSlot(int day, string time)
        {
            string userId = _userManager.GetUserId(User);

            // Convert the string time to a TimeSpan
            if (!TimeSpan.TryParse(time, out TimeSpan timeOfDay))
            {
                return BadRequest("Invalid time format");
            }

          
            DateTime baseDate = new DateTime(2024, 4, 29); // Random constant date (monday)

            // Calculate the offset for the specified day of the week
            int dayOffset = day - (int)baseDate.DayOfWeek;
            if (dayOffset < 0)
            {
                dayOffset += 7; 
            }

          
            DateTime beginTime = baseDate.AddDays(dayOffset).Add(timeOfDay);

            
            TimeSlot timeSlot = new TimeSlot
            {
                BeginTime = beginTime,
                EmployeeID = userId,
                TicketID = 0,
                IsAvailable = true
            };


            _context.TimeSlot.Add(timeSlot);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSlot(string id)
        {

            TimeSlot timeSlot = await _context.TimeSlot.FindAsync(id);

            if (timeSlot != null)
            {
                _context.TimeSlot.Remove(timeSlot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

       
        
    }
}
