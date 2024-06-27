using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Data;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Identity;
using CarWorkshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using CarWorkshop.ViewModels;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class TicketsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;

        public TicketsController(AuthDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ticket.ToListAsync());
        }

      
        
        public IActionResult EmployeeTickets()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var tickets = _context.Ticket.Where(t => t.EmployeeID == user.Id).ToList();
            return View(tickets);
        }


        
        

//: Tickets/Details/5
        public async Task<IActionResult> Details(int id, DateTime? selectedDate)
        {
            DateTime currentDate = selectedDate ?? DateTime.Today;
            if (id == 0)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            var parts = await _context.BoughtPart.Where(p => p.TicketId == id).ToListAsync();
            DateTime sunday = currentDate.AddDays(-(((int)currentDate.DayOfWeek + 7) % 7));
            DateTime saturday = sunday.AddDays(6).Date;

            // Call the GenerateWeekTable method on the instance
            var weektable = GenerateWeekTable(id, sunday, saturday);

            var viewModel = new DetailsViewModel(ticket, weektable, parts, currentDate);

            return View(viewModel);
        }


        private List<TimeSlot> GetTimeSlotsForDate(int ticketID, DateTime date)
        {
            List<TimeSlot> timeSlots = _context.TimeSlot.Where(ts => ts.TicketID == ticketID && ts.BeginTime.Date == date.Date).ToList();

            return timeSlots;
        }

        private WeeklySchedule GenerateWeekTable(int tID, DateTime startDate, DateTime endDate)
        {
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

            return weekSchedule;
        }












        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Brand,Model,RegistrationID,Description")] Ticket ticket)
        {
            foreach (var key in ModelState.Keys)
            {
                var entry = ModelState[key];
                if (entry.Errors.Any())
                {
                    foreach (var error in entry.Errors)
                    {
                        Console.WriteLine($"Validation error for field '{key}': {error.ErrorMessage}");
                    }
                }
            }

       
            ticket.EmployeeID = "";
            ticket.EstimateDescription = "";
            ticket.EstimateCost = 0;
            ticket.EstimateAcceptedByClient = false;
            ticket.PaidByClient = 0;
            ticket.State = "Created";
           
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("Ticket created");
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Tickets");
            }
            return View(ticket);
        }



       


        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            List<BoughtPart> parts = await _context.BoughtPart.Where(p => p.TicketId == id).ToListAsync();
            
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            
          

            if (ticket == null)
            {
                return NotFound();
            }

            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != ticket.EmployeeID || ticket.State == "Closed")
            {
                return RedirectToAction("Index", "Home");
            }

            EditViewModel viewModel = new EditViewModel
            {
                Ticket = ticket,
                Parts = parts
            };

            return View(viewModel);
        }





        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,RegistrationID,Description,EstimateDescription,EstimateCost,EstimateAcceptedByClient,PaidByClient")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    var existingTicket = await _context.Ticket.FindAsync(id);
                    if (existingTicket == null)
                    {
                        return NotFound();
                    }
                    

                    if(existingTicket.EmployeeID != _userManager.GetUserId(User))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    
                    ticket.EmployeeID = existingTicket.EmployeeID;
                if (ticket.PaidByClient > 0)
                {
                    ticket.State = "Done";
                }
                else
                {
                    ticket.State = "In Progress";
                }

                _context.Entry(existingTicket).CurrentValues.SetValues(ticket);

                    await _context.SaveChangesAsync();
                
                    return RedirectToAction("EmployeeTickets", "Employee");
            }
            return View(ticket);
        }




        public async Task<IActionResult> DeletePart(int id)
        {
           
            var boughtPart = await _context.BoughtPart.FindAsync(id);
            int ticketId = 0;
            if (boughtPart != null)
            {
                _context.BoughtPart.Remove(boughtPart);
                ticketId = boughtPart.TicketId;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Tickets", new { id = ticketId });
   
        }


    








        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                _context.Ticket.Remove(ticket);
            }
            _context.BoughtPart.RemoveRange(_context.BoughtPart.Where(p => p.TicketId == id));
            _context.TimeSlot.RemoveRange(_context.TimeSlot.Where(ts => ts.TicketID == id));

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }




        
        public async Task<IActionResult> Accept(int? id)
        {
           
            var loggedInUser = await _userManager.GetUserAsync(User);

            if (loggedInUser == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.EmployeeID = loggedInUser.Id;
            if(ticket.State == "Created")
            {
                ticket.State = "In Progress";
            }
            

          
            await _context.SaveChangesAsync();

           
            return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
        }


        public async Task<IActionResult> Close(int? id)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            if (loggedInUser == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }
            if (ticket.State == "Done")
            {
                ticket.State = "Closed";
                _context.RemoveRange(_context.TimeSlot.Where(ts => ts.TicketID == ticket.Id));
            }
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
        }
    }



}

