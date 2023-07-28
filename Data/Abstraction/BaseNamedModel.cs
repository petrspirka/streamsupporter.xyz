using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data.Abstraction
{
    public class BaseNamedModel : BaseOwnedModel
    {
        [MaxLength(32)]
        [Display(Name = "Name", Description = "Name of this widget. With normal widgets used for categorization, rewards use this as a header.")]
        public string Name { get; set; } = default!;
    }
}
