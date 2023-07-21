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
        public ulong Cost { get; set; } = 1000;
        [ValidateNever]
        [Display(Name = "Triggered component")]
        public string? TriggeredId { get; set; } = default!;
        [ValidateNever]
        public string? TriggeredType { get; set; } = default!;
        [Display(Name = "Can users enter messages?")]
        public bool HasTextField { get; set; } = false;
        [Display(Name = "Should the reward be automatically accepted?")]
        public bool AutoAccept { get; set; } = false;
    }
}
