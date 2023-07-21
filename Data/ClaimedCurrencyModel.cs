using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    [Table("ClaimedCurrency")]
    public class ClaimedCurrencyModel : BaseOwnedModel
    {
        [ForeignKey(nameof(ShopOwner))]
        public string ShopOwnerId { get; set; } = default!;
        public ApplicationUser ShopOwner { get; set; } = default!;
        public ulong Currency { get; set; } = default!;
    }
}
