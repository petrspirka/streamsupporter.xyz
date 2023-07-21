using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    [Table("Purchases")]
    public class PurchaseModel : BaseOwnedModel
    {
        public ApplicationUser Buyer { get; set; } = default!;
        [ForeignKey(nameof(Buyer))]
        public string BuyerId { get; set; } = default!;

        public RewardModel Reward { get; set; } = default!;
        [ForeignKey(nameof(Reward))]
        public string RewardId { get; set; } = default!;
        public string? Text { get; set; } = default!;
        public ulong CostAtPurchase { get; set; } = default!;
        public bool Finished { get; set; } = false;
        public bool Confirmed { get; set; } = false;
    }
}
