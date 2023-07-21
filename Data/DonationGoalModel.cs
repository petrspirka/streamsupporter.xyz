using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    public class DonationGoalModel : BaseComponentModel
    {
        [Display(Name = "Target amount")]
        [Column(TypeName = "decimal(9, 2)")]
        public decimal TargetAmount { get; set; } = default!;
        [Display(Name = "Current amount")]
        [Column(TypeName = "decimal(9, 2)")]
        public decimal CurrentAmount { get; set; } = default!;
        [Display(Name = "Expiry")]
        [Column(TypeName = "Date")]
        public DateOnly ExpiryDate { get; set; } = default!;
    }
}
