using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewStreamSupporter.Data;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<MarqueeModel> Marquees { get; set; } = default!;
    public DbSet<RewardModel> Rewards { get; set; } = default!;
    public DbSet<AlertModel> Alerts { get; set; } = default!;
    public DbSet<UnclaimedCurrencyModel> UnclaimedCurrencies { get; set; } = default!;
    public DbSet<ClaimedCurrencyModel> ClaimedCurrencies { get; set; } = default!;
    public DbSet<PurchaseModel> Purchases { get; set; } = default!;
    public DbSet<NotificationModel> Notifications { get; set; } = default!;
    public DbSet<StreamerFollow> StreamerFollows { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.OwnedMarquees)
            .WithOne(m => m.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Purchases)
            .WithOne(p => p.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.OwnedRewards)
            .WithOne(m => m.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.OwnedDonationGoals)
            .WithOne(d => d.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.OwnedTimers)
            .WithOne(t => t.Owner);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.OwnedCounters)
            .WithOne(c => c.Owner);

        builder.Entity<ClaimedCurrencyModel>()
            .HasAlternateKey(c => new { c.ShopOwnerId, c.OwnerId });
    }

    public DbSet<NewStreamSupporter.Data.CounterModel> CounterModel { get; set; } = default!;

    public DbSet<NewStreamSupporter.Data.DonationGoalModel> DonationGoalModel { get; set; } = default!;

    public DbSet<NewStreamSupporter.Data.TimerModel> TimerModel { get; set; } = default!;
}
