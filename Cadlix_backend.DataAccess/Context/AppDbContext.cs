using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.History;
using Cadlix_backend.Domain.Entities.Leaderboard;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.Movie;
using Cadlix_backend.Domain.Entities.Subscription;
using Cadlix_backend.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Context;

public class AppDbContext : DbContext
{
    public DbSet<CategoryData> Categories { get; set; }

    public DbSet<HistoryData> Histories { get; set; }

    public DbSet<LeaderboardData> Leaderboards { get; set; }

    public DbSet<ListsData> Lists { get; set; }

    public DbSet<MovieData> Movies { get; set; }

    public DbSet<SubscriptionData> Subscriptions { get; set; }

    public DbSet<UserData> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(DbSession.ConnectionString);
    }
}
