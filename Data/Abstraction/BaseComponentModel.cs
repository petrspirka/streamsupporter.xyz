using NewStreamSupporter.Data.Abstraction;
using NewStreamSupporter.Models;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data.Abstractions
{
    public abstract class BaseComponentModel : BaseNamedModel
    {
        [MaxLength(256)]
        public string Text { get; set; } = "";

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Background color")]
        public string BackgroundColor { get; set; } = "#FFFFFFFF";

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Font color")]
        public string FontColor { get; set; } = "#000000FF";
    }
}
