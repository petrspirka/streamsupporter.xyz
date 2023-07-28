using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    public class DonationGoalModel : BaseComponentModel
    {
        [Display(Name = "Target amount", Description = "The amount representing maximum of the donation goal. Money will accumulate beyond this point but the donation goal will always look full beyond this value.")]
        [Column(TypeName = "decimal(9, 2)")]
        [Range(0, 9999999.99)]
        public decimal TargetAmount { get; set; } = default!;
        [Display(Name = "Current amount", Description = "Current amount in this donation goal model. It increases whenever a donation is detected.")]
        [Column(TypeName = "decimal(9, 2)")]
        [Range(0, 9999999.99)]
        public decimal CurrentAmount { get; set; } = default!;
        [Display(Name = "Expiry", Description = "The time displayed at which the donation goal expires. Expiry has no effect on the function of the widget and simply lets you keep accepting donations after expiry.")]
        [Column(TypeName = "Date")]
        public DateOnly ExpiryDate { get; set; } = default!;
    }
}
