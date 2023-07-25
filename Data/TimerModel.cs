using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data
{
    public class TimerModel : BaseComponentModel, IRewardTriggerable
    {
        [Display(Name = "Timer length (in seconds)")]
        [Range(1.0, 35999.0)]
        public ulong Length { get; set; } = default!;
    }
}
