using CarWorkshop.Models;

namespace CarWorkshop.ViewModels
{
    public class DetailsViewModel
    {
        
        public Ticket TicketData { get; set; }
        public WeeklySchedule TicketTimeTable { get; set; }
        public List<BoughtPart> PartsData { get; set; }

        public DateTime Date { get; set; }

        public DetailsViewModel(Ticket ticket, WeeklySchedule ticketSchedule, List<BoughtPart> parts, DateTime date)
        {
            TicketData = ticket;
            TicketTimeTable = ticketSchedule;
            PartsData = parts;
            Date = date;
        }

    }
}

