﻿using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    public class DonationGoalModel : BaseComponentModel
    {

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Progress bar color", Description = "The color of the progress bar of this widget.")]
        public new string BackgroundColor { get; set; } = "#FFFFFFFF";
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
