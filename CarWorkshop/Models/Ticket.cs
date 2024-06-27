using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        public string? Brand { get; set; }
        [Required]
        public string? Model { get; set; }
        [Required]

        [Display(Name = "License Plate")]
        public string? RegistrationID { get; set; }
        [Required]
        public string? Description { get; set; }

        public string? EmployeeID { get; set; }

        [Display(Name = "Estimate Description")]
        public string? EstimateDescription { get; set; }
        [Range(0, float.MaxValue)]

        [Display(Name = "Estimate Cost ($)")]
        public float EstimateCost { get; set; }

        [Display(Name = "Accepted by client?")]
        public bool EstimateAcceptedByClient { get; set; }
        [Range(0, float.MaxValue)]

        [Display(Name = "Paid by client ($)")]
        public float PaidByClient { get; set; }

        public string? State { get; set; }
    }
}
