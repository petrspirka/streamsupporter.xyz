using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data.Abstractions
{
    public abstract class BaseModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ValidateNever]
        public string Id { get; set; } = default!;
    }
}
