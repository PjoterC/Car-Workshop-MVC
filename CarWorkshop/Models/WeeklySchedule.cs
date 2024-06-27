using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class WeeklySchedule
    {
        [Key]
        public string? EmployeeID { get; set; }
        public List<TimeSlot> Monday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Tuesday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Wednesday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Thursday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Friday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Saturday { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> Sunday { get; set; } = new List<TimeSlot>();

        public List<TimeSlot> GetDayByIndex(int index)
        {
            return index switch
            {
                1 => Monday,
                2 => Tuesday,
                3 => Wednesday,
                4 => Thursday,
                5 => Friday,
                6 => Saturday,
                0 => Sunday,
                _ => new List<TimeSlot>()
            };
        }

        public string GetDayName(int index)
        {
            return index switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                0 => "Sunday",
                _ => ""
            };
        }

    }
}
