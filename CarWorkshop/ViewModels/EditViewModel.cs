using CarWorkshop.Models;

namespace CarWorkshop.ViewModels
{
    public class EditViewModel
    {
        public Ticket Ticket { get; set; }
        public List<BoughtPart> Parts { get; set; }
    }
}
