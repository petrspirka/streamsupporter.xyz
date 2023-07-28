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
    public AlertModel Alert { get; set; } = new();

    [PersonalData]
    public string? TwitchId { get; set; }
    [PersonalData]
    public string? TwitchUsername { get; set; }
    [PersonalData]
    public string? GoogleBrandId { get; set; }
    [PersonalData]
    public string? GoogleBrandName { get; set; }

    public bool IsGoogleActive { get; set; } = default!;
    public bool IsTwitchActive { get; set; } = default!;
    [PersonalData]
    public string? TwitchRefreshToken { get; set; }
    [PersonalData]
    public string? TwitchAccessToken { get; set; }
    [PersonalData]
    public DateTime? TwitchAccessTokenExpiry { get; set; }
}
