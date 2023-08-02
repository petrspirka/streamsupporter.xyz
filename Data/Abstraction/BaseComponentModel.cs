using NewStreamSupporter.Data.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data.Abstractions
{
    public abstract class BaseComponentModel : BaseNamedModel
    {
        [MaxLength(256)]
        [Display(Name = "Text", Description = "Text that will be displayed by this widget.")]
        public string Text { get; set; } = "";

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Background color", Description = "The background color of this widget.")]
        public string BackgroundColor { get; set; } = "#FFFFFFFF";

        [RegularExpression(@"(\#)([0-9]|[A-F]){8}", ErrorMessage = "The value must be a valid hexa color (f.e. \"#FF0000FF\")")]
        [MaxLength(9)]
        [Display(Name = "Font color", Description = "The font color of this widget")]
        public string FontColor { get; set; } = "#000000FF";
    }
}
