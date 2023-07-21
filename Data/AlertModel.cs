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

        [Display(Name = "Do follows trigger alert?")]
        public bool ShouldTriggerFollows { get; set; } = false;

        [Display(Name = "Do donations trigger alert?")]
        public bool ShouldTriggerDonations { get; set; } = false;

        [Display(Name = "Duration")]
        [Range(1.0, 15.0)]
        public float AlertDuration { get; set; } = 5.0f;
    }
}
