using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data.Abstractions
{
    public abstract class BaseOwnedModel : BaseModel
    {
        [ForeignKey(nameof(Owner))]
        [ValidateNever]
        public string OwnerId { get; set; } = default!;

        [ValidateNever]
        public ApplicationUser Owner { get; set; } = default!;
    }
}
