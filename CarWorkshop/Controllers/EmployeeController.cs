using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
            
        }

        public IActionResult AllTickets()
        {
            return RedirectToAction("Index", "Tickets");
         
        }

        public IActionResult EmployeeTickets()
        {
            
            return RedirectToAction("EmployeeTickets", "Tickets");
        
        }
        public IActionResult EmployeeSchedule()
        {
            return RedirectToAction("Index", "EmployeeScheduleEdit");
          
        }


        


    }
}
