using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data
{
    public class CounterModel : BaseComponentModel, IRewardTriggerable
    {
        [Display(Name = "Value", Description = "Value of the counter model that is displayed under the text. Automatically increased by rewards.")]
        [Range(-999999999.0, 999999999.0)]
        public long Value { get; set; } = default!;

        [Display(Name = "Should donations increase value?", Description = "Should this counter be increased when a donation is received?")]
        public bool TriggeredByDonations { get; set; } = false;
    }
}
