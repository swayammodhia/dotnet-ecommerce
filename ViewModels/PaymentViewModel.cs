using System.ComponentModel.DataAnnotations;

namespace ElectronyatShop.ViewModels
{
    public class PaymentViewModel
    {
        public int CartId { get; set; }
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "Card number must be between 13 and 16 digits")]
        public string CardNumber { get; set; }

        /*[Required]
        [Display(Name = "Expiry Month")]
        [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Enter valid month (01-12)")]
        public string ExpiryMonth { get; set; }

        [Required]
        [Display(Name = "Expiry Year")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Enter valid year (e.g. 2025)")]
        public string ExpiryYear { get; set; }*/

        /*[Required]
        [Display(Name = "Expiry (Month/Year)")]
        [DataType(DataType.Date)]
        public DateTime ExpiryMonthYear { get; set; }*/

        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(2020, 2035)]  // Adjust max year as needed
        public int ExpiryYear { get; set; }


        [Required]
        [Display(Name = "CVC")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVC must be 3 or 4 digits")]
        public string CVC { get; set; }
    }
}
