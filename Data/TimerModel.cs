using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data
{
    public class TimerModel : BaseComponentModel, IRewardTriggerable
    {

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Progress bar color", Description = "The color of the progress bar of this widget.")]
        public new string BackgroundColor { get; set; } = "#FFFFFFFF";

        [Display(Name = "Timer length", Description = "Describes how long the timer should run for in seconds.")]
        [Range(1.0, 35999.0)]
        public ulong Length { get; set; } = default!;
    }
}
