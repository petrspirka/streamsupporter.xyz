using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    [Table("Rewards")]
    public class RewardModel : BaseComponentModel
    {
        [Range(1.0, 999999999.0)]
        [Required]
        [Display(Name = "Cost", Description = "The amount of points needed to purchase this reward.")]
        public ulong Cost { get; set; } = 1000;
        [ValidateNever]
        [Display(Name = "Triggered widget", Description = "The widget that shuold be activated by this reward. Leave at default to not trigger any.")]
        public string? TriggeredId { get; set; } = default!;
        [ValidateNever]
        public string? TriggeredType { get; set; } = default!;
        [Display(Name = "Can users enter messages?", Description = "Enables users to enter a message into the reward when purchasing. This is used with the widgets specified by the \"Triggered widget\" property.")]
        public bool HasTextField { get; set; } = false;
        [Display(Name = "Should the reward be automatically accepted?", Description = "By default, all rewards have to be accepted in the user dashboard. Enabling this option makes rewards automatically accepted.")]
        public bool AutoAccept { get; set; } = false;
    }
}
