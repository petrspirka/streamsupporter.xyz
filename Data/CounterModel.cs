using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data
{
    public class CounterModel : BaseComponentModel, IRewardTriggerable
    {
        [Display(Name = "Value")]
        [Range(-999999999.0, 999999999.0)]
        public long Value { get; set; } = default!;

        [Display(Name = "Should donations increase value?")]
        public bool TriggeredByDonations { get; set; } = false;
    }
}
