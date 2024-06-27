using CarWorkshop.Areas.Identity.Data;
using CarWorkshop.Data;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeScheduleController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;

        public EmployeeScheduleController(AuthDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(string? userName ,DateTime? selectedDate)
        {
            User user = null;
            if (userName != null)
            {
                user = _userManager.FindByNameAsync(userName).Result;
            }
            
            if (user == null)
            {
                user = _userManager.GetUserAsync(User).Result;
            }

            DateTime currentDate = selectedDate ?? DateTime.Today;

            DateTime sunday = currentDate.AddDays(-(((int)currentDate.DayOfWeek + 7) % 7)); ;
            DateTime saturday;

            saturday = sunday.AddDays(6);

            saturday = saturday.Date;
            sunday = sunday.Date;


            // Generate the week table based on the start and end dates
            var model = GenerateWeekTable(user.Id, sunday, saturday);
            System.Tuple<WeeklySchedule, DateTime> tuple = new System.Tuple<WeeklySchedule, DateTime>(model, currentDate);

            return View(tuple);

        }

        private WeeklySchedule GenerateWeekTable(string uID, DateTime startDate, DateTime endDate)
        {
            string userId = uID;

            WeeklySchedule weekSchedule = new WeeklySchedule();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {

                var timeSlots = GetTimeSlotsForDate(userId, date);


                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weekSchedule.Monday = timeSlots;
                        break;
                    case DayOfWeek.Tuesday:
                        weekSchedule.Tuesday = timeSlots;
                        break;
                    case DayOfWeek.Wednesday:
                        weekSchedule.Wednesday = timeSlots;
                        break;
                    case DayOfWeek.Thursday:
                        weekSchedule.Thursday = timeSlots;
                        break;
                    case DayOfWeek.Friday:
                        weekSchedule.Friday = timeSlots;
                        break;
                    case DayOfWeek.Saturday:
                        weekSchedule.Saturday = timeSlots;
                        break;
                    case DayOfWeek.Sunday:
                        weekSchedule.Sunday = timeSlots;
                        break;
                }
            }
            weekSchedule.Monday ??= new List<TimeSlot>();
            weekSchedule.Tuesday ??= new List<TimeSlot>();
            weekSchedule.Wednesday ??= new List<TimeSlot>();
            weekSchedule.Thursday ??= new List<TimeSlot>();
            weekSchedule.Friday ??= new List<TimeSlot>();
            weekSchedule.Saturday ??= new List<TimeSlot>();
            weekSchedule.Sunday ??= new List<TimeSlot>();

            return weekSchedule;
        }

        private List<TimeSlot> GetTimeSlotsForDate(string userId, DateTime date)
        {
            return _context.TimeSlot.AsEnumerable().Where(ts => ts.EmployeeID == userId && ts.BeginTime.Date == date && ts.TicketID != 0).ToList();
        }
    }
}
