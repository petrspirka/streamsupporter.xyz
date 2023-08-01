using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    [Table("Alerts")]
    public class AlertModel : BaseComponentModel, IRewardTriggerable
    {
        //Each user has only 1 alert, therefore all alerts will have the same name
        [NotMapped]
        public new string Name { get; set; } = "Alert";
        [NotMapped]
        public new string Text { get; set; } = "";

        [Display(Name = "Do follows trigger alert?", Description = "Whether this alert should display a new follow message when a new follower is detected.")]
        public bool ShouldTriggerFollows { get; set; } = false;

        [Display(Name = "Do donations trigger alert?", Description = "Whether this alert should display donation message when a new donation is detected.")]
        public bool ShouldTriggerDonations { get; set; } = false;

        [Display(Name = "Duration", Description = "How long the alert stays on screen.")]
        [Column(TypeName = "decimal(5, 2)")]
        [Range(1.0, 30.0)]
        public decimal AlertDuration { get; set; } = 5.0M;
        [ValidateNever]
        public string? AudioType { get; set; }
    }
}
