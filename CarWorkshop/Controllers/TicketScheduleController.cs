using CarWorkshop.Areas.Identity.Data;
using CarWorkshop.Data;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class TicketScheduleController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        public TicketScheduleController(AuthDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int ticketID, DateTime? selectedDate)
        {

            var ticket = _context.Ticket.Find(ticketID);
            var user = _userManager.GetUserAsync(User).Result;

            if (ticket == null || user == null)
            {
                   return RedirectToAction("Index", "Home");
            }

            if (user.Id != ticket.EmployeeID)
            {
                return RedirectToAction("Index", "Home");
            }

            // If no date is selected, default to today's date
            DateTime currentDate = selectedDate ?? DateTime.Today;

            DateTime sunday = currentDate.AddDays(-(((int)currentDate.DayOfWeek + 7) % 7)); ;
            DateTime saturday;
    
            saturday = sunday.AddDays(6);
            
            saturday = saturday.Date;
            sunday = sunday.Date;


            // Generate the week table based on the start and end dates
            var model = GenerateWeekTable(user.Id, ticketID, sunday, saturday);
            model.Date = currentDate;

            return View(model);
        }

        private TicketSchedule GenerateWeekTable(string uID, int tID, DateTime startDate, DateTime endDate)
        {
            string userId = uID;
            int tickedID = tID;
            

            WeeklySchedule weekSchedule = new WeeklySchedule();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {

                var timeSlots = GetTimeSlotsForDate(tID, date);


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

            var takenTimeSlots = _context.TimeSlot.Where(ts => ts.TicketID != tID && ts.TicketID != 0 && ts.EmployeeID == userId).ToList();

            var employeeScheduleSlots = _context.TimeSlot.Where(ts => (ts.TicketID == 0 && ts.EmployeeID == userId)).ToList();
           
            var ticketSchedule = new TicketSchedule
            {
                employeeID = userId,
                ticketID = tID,
                Date = startDate,
                takenTimeSlots = takenTimeSlots,
                employeeScheduleSlots = employeeScheduleSlots,
                WeeklySchedule = weekSchedule
            };

            return ticketSchedule;
        }

        private List<TimeSlot> GetTimeSlotsForDate(int ticketID, DateTime date)
        {
            List<TimeSlot> timeSlots = _context.TimeSlot.Where(ts => ts.TicketID == ticketID && ts.BeginTime.Date == date.Date).ToList();
            
            return timeSlots;
        }

        [HttpPost]
        public IActionResult AddSlot(int ticketID, int day, string time, DateTime selectedDate)
        {
            
            var user = _userManager.GetUserAsync(User).Result;
            string userId = user.Id;

            if (!TimeSpan.TryParse(time, out TimeSpan timeOfDay))
            {
                return BadRequest("Invalid time format");
            }

            
            DateTime sunday = selectedDate.AddDays(-(int)selectedDate.DayOfWeek);
       
            DateTime slotTime = sunday.AddDays(day).Add(timeOfDay); 
           

            // Check if the selected slot is already taken
            bool slotTaken = _context.TimeSlot.Any(ts => ts.TicketID != ticketID && ts.EmployeeID == userId && ts.TicketID != 0 && ts.BeginTime == slotTime);
            if (slotTaken)
            {
                return BadRequest("Slot is already taken");
            }


            TimeSlot newSlot = new TimeSlot
            {
                EmployeeID = userId,
                BeginTime = slotTime,
                TicketID = ticketID
            };


            _context.TimeSlot.Add(newSlot);
            _context.SaveChanges();


            return RedirectToAction("Index", new { ticketID, selectedDate});
        }


    

        public async Task<IActionResult> DeleteSlot(string id)
        {

            TimeSlot timeSlot = await _context.TimeSlot.FindAsync(id);

            DateTime selectedDate = timeSlot.BeginTime;
            int ticketID = timeSlot.TicketID;
            if (timeSlot != null)
            {
                _context.TimeSlot.Remove(timeSlot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { ticketID, selectedDate});
        }
    }
}
