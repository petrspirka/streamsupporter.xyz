using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    [Table("UnclaimedCurrency")]
    public class UnclaimedCurrencyModel : BaseOwnedModel
    {
        public string? GoogleId { get; set; }
        public string? TwitchId { get; set; }
        public ulong Currency { get; set; } = default!;
    }
}
