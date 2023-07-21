using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Data.Abstraction
{
    public class BaseNamedModel : BaseOwnedModel
    {
        [MaxLength(32)]
        public string Name { get; set; } = default!;
    }
}
