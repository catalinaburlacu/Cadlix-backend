namespace Cadlix_backend.Domain.DTOs.Frontend;

public class UserStatsDto
{
    public double Score { get; set; }
    public int TitlesWatched { get; set; }
    public int Comments { get; set; }
    public int LikesGiven { get; set; }
    public int LikesReceived { get; set; }
    public int HoursWatched { get; set; }
    public int AddedToList { get; set; }
    public int DaysOnSite { get; set; }
}

public class WatchListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string Episode { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public string Poster { get; set; } = string.Empty;
}

public class WatchHistoryItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Episode { get; set; } = string.Empty;
    public DateTime WatchedAt { get; set; }
    public string Progress { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public UserStatsDto Stats { get; set; } = new();
    public List<WatchListItemDto> WatchList { get; set; } = new();
    public List<WatchHistoryItemDto> WatchHistory { get; set; } = new();
}
