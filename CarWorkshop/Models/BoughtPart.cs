using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class BoughtPart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public int TicketId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]

        [Display(Name = "Price ($)")]
        public float Price { get; set; }

        /*float TotalPrice { get; }

        float CalculateTotalPrice()
        {
            return Amount * Price;
        }

        public BoughtPart()
        {
            TotalPrice = CalculateTotalPrice();
        }*/
    }
}
