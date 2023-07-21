using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data
{
    public class MarqueeModel : BaseComponentModel, IRewardTriggerable
    {
        [Range(0.0, 20.0)]
        [Display(Name = "Duration")]
        public float SpeedFactor { get; set; } = 1;
        [Range(0.0, 5.0)]
        [Display(Name = "Duration per character")]
        public float SpeedFactorPerCharacter { get; set; } = 0.1f;
        [Display(Name = "Fade animation length")]
        [Range(0.0, 10.0)]
        public float FadeTime { get; set; } = 1;
        [Display(Name = "Delay")]
        [Range(0.0, 86400.0)]
        public float Delay { get; set; } = 0;
        [Display(Name = "Permanent?")]
        public bool Permanent { get; set; } = false;
    }
}
