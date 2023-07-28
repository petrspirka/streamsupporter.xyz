using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    public class MarqueeModel : BaseComponentModel, IRewardTriggerable
    {
        [Range(0.0, 20.0)]
        [Display(Name = "Duration", Description = "The base duration of the marquee in seconds.")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal SpeedFactor { get; set; } = 1.0M;
        [Range(0.0, 5.0)]
        [Display(Name = "Duration per character", Description = "How many seconds should be added to the base duration per every character in the text.")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal SpeedFactorPerCharacter { get; set; } = 0.1M;
        [Display(Name = "Fade animation length", Description = "Length of the fade-in and fade-out animations displayed when the marquee activates. Counted separate from base duration specified by the \"Duration\" and \"Duration per character\" properties.")]
        [Column(TypeName = "decimal(4, 2)")]
        [Range(0.0, 10.0)]
        public decimal FadeTime { get; set; } = 1;
        [Display(Name = "Delay", Description = "Takes effect when marquee is in permanent mode. Specifies time between activations.")]
        [Column(TypeName = "decimal(7, 2)")]
        [Range(0.0, 86400.0)]
        public float Delay { get; set; } = 0;
        [Display(Name = "Permanent?", Description = "Specifies whether the marquee should keep cycling the \"Text\" property continuously.")]
        public bool Permanent { get; set; } = false;
    }
}
