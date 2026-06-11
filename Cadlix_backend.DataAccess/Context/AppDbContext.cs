using System.Security.Cryptography;
using System.Text;
using Cadlix_backend.Domain.Entities.Categories;
using Cadlix_backend.Domain.Entities.History;
using Cadlix_backend.Domain.Entities.Leaderboard;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.Movie;
using Cadlix_backend.Domain.Entities.Review;
using ReviewLikeData = Cadlix_backend.Domain.Entities.Review.ReviewLikeData;
using Cadlix_backend.Domain.Entities.Subscription;
using Cadlix_backend.Domain.Entities.User;
using UserLikeData = Cadlix_backend.Domain.Entities.User.UserLikeData;
using RefreshTokenData = Cadlix_backend.Domain.Entities.User.RefreshTokenData;
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

    public DbSet<ReviewData> Reviews { get; set; }

    public DbSet<UserLikeData> UserLikes { get; set; }

    public DbSet<ReviewLikeData> ReviewLikes { get; set; }

    public DbSet<RefreshTokenData> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(DbSession.ConnectionString);
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between MovieData and CategoryData
        modelBuilder.Entity<MovieData>()
            .HasMany(m => m.Genres)
            .WithMany(c => c.Movies)
            .UsingEntity(j => j.ToTable("MovieGenres"));

        // Indexes for search performance
        modelBuilder.Entity<MovieData>()
            .HasIndex(m => m.Title)
            .HasDatabaseName("IX_Movies_Title");

        modelBuilder.Entity<MovieData>()
            .HasIndex(m => m.Director)
            .HasDatabaseName("IX_Movies_Director");

        modelBuilder.Entity<MovieData>()
            .HasIndex(m => m.Description)
            .HasDatabaseName("IX_Movies_Description");

        modelBuilder.Entity<SubscriptionData>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MovieData>()
            .Property(m => m.VideoSources)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>?>(v, (System.Text.Json.JsonSerializerOptions?)null));

        modelBuilder.Entity<MovieData>()
            .Property(m => m.Score)
            .HasConversion<double?>();

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

        modelBuilder.Entity<UserLikeData>()
            .HasIndex(ul => new { ul.LikerId, ul.LikedUserId })
            .IsUnique()
            .HasDatabaseName("IX_UserLikes_LikerId_LikedUserId");

        modelBuilder.Entity<UserLikeData>()
            .HasOne(ul => ul.Liker)
            .WithMany()
            .HasForeignKey(ul => ul.LikerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserLikeData>()
            .HasOne(ul => ul.LikedUser)
            .WithMany()
            .HasForeignKey(ul => ul.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewLikeData>()
            .HasIndex(rl => new { rl.ReviewId, rl.UserId })
            .IsUnique()
            .HasDatabaseName("IX_ReviewLikes_ReviewId_UserId");

        modelBuilder.Entity<ReviewLikeData>()
            .HasOne(rl => rl.Review)
            .WithMany(r => r.ReviewLikes)
            .HasForeignKey(rl => rl.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewLikeData>()
            .HasOne(rl => rl.User)
            .WithMany()
            .HasForeignKey(rl => rl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReviewData>()
            .Property(r => r.LikesCount)
            .HasDefaultValue(0);

        modelBuilder.Entity<ReviewData>()
            .HasIndex(r => new { r.UserId, r.MovieId })
            .IsUnique()
            .HasDatabaseName("IX_Reviews_UserId_MovieId");

        modelBuilder.Entity<RefreshTokenData>()
            .HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        modelBuilder.Entity<RefreshTokenData>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CategoryData>().HasData(genres);
        modelBuilder.Entity<UserData>().HasData(users);
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
