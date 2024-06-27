using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class TimeSlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BeginTime { get; set; }


        public string? EmployeeID { get; set; }

        public int TicketID { get; set; }

        public bool IsAvailable { get; set; } = true;


    }
}
