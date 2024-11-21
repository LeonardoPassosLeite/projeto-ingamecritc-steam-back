using Microsoft.EntityFrameworkCore;

public class SteamChartsDbContext : DbContext
{
    public SteamChartsDbContext(DbContextOptions<SteamChartsDbContext> options) : base(options)
    { }

    public DbSet<HourlyGame> HourlyGames { get; set; }
}