using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class TicketSchedule
    {
        public int ticketID { get; set; }
        public string? employeeID { get; set; }

        public DateTime Date { get; set; }
        public List<TimeSlot> takenTimeSlots { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> employeeScheduleSlots { get; set; } = new List<TimeSlot>();
        public WeeklySchedule WeeklySchedule { get; set; } = new WeeklySchedule();

        

    }
}
