using System.Security.Cryptography;
using System.Text;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between MovieData and CategoryData
        modelBuilder.Entity<MovieData>()
            .HasMany(m => m.Genres)
            .WithMany(c => c.Movies)
            .UsingEntity(j => j.ToTable("MovieGenres"));

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes("admin123");
        var hash = sha256.ComputeHash(bytes);

        var users = new[]
        {
            new UserData
            {
                Id = 1,
                Name = "Admin",
                Password = Convert.ToBase64String(hash),
                Email = "admin@cadlix.com",
                Group = "Admin",
                Level = URole.Admin,
                Plan = "Premium",
                Status = "Active",
                TitlesWatched = 0,
                ReviewCount = 0,
                Rating = null,
                Comments = 0,
                LikesGiven = 0,
                LikesReceived = 0,
                HoursWatched = 0,
                AddedToList = 0,
                DaysOnSite = 0
            },
        };


        // Seed predefined genres (categories)
        var genres = new[]
        {
            new CategoryData { Id = 1, Name = "Action", Title = "Action" },
            new CategoryData { Id = 2, Name = "Comedy", Title = "Comedy" },
            new CategoryData { Id = 3, Name = "Drama", Title = "Drama" },
            new CategoryData { Id = 4, Name = "Horror", Title = "Horror" },
            new CategoryData { Id = 5, Name = "Romance", Title = "Romance" },
            new CategoryData { Id = 6, Name = "Sci-Fi", Title = "Science Fiction" },
            new CategoryData { Id = 7, Name = "Thriller", Title = "Thriller" },
            new CategoryData { Id = 8, Name = "Animation", Title = "Animation" },
            new CategoryData { Id = 9, Name = "Fantasy", Title = "Fantasy" },
            new CategoryData { Id = 10, Name = "Adventure", Title = "Adventure" },
            new CategoryData { Id = 11, Name = "Mystery", Title = "Mystery" },
            new CategoryData { Id = 12, Name = "Crime", Title = "Crime" },
            new CategoryData { Id = 13, Name = "Documentary", Title = "Documentary" },
            new CategoryData { Id = 14, Name = "Musical", Title = "Musical" },
            new CategoryData { Id = 15, Name = "Family", Title = "Family" }
        };

        modelBuilder.Entity<CategoryData>().HasData(genres);
        modelBuilder.Entity<UserData>().HasData(users);
    }
}
