using Microsoft.AspNetCore.Identity;

namespace NewStreamSupporter.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ICollection<MarqueeModel> OwnedMarquees { get; set; } = default!;
    public ICollection<RewardModel> OwnedRewards { get; set; } = default!;
    public ICollection<DonationGoalModel> OwnedDonationGoals { get; set; } = default!;
    public ICollection<TimerModel> OwnedTimers { get; set; } = default!;
    public ICollection<CounterModel> OwnedCounters { get; set; } = default!;
    public ICollection<PurchaseModel> Purchases { get; set; } = default!;
    public ICollection<NotificationModel> Notifications { get; set; } = default!;
    public AlertModel Alert { get; set; } = default!;

    public string? TwitchId { get; set; }
    public string? TwitchUsername { get; set; }
    public string? GoogleBrandId { get; set; }
    public string? GoogleBrandName { get; set; }

    public bool IsGoogleActive { get; set; } = default!;
    public bool IsTwitchActive { get; set; } = default!;
    public string? TwitchRefreshToken { get; set; }
    public string? TwitchAccessToken { get; set; }
    public DateTime? TwitchAccessTokenExpiry { get; set; }
}
