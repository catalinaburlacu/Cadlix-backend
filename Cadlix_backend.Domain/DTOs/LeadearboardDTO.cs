using System;

namespace Cadlix_backend.Domain.DTOs;

public class LeadearboardDTO
{
    public int Rank { get; set; }
    public int RankChange { get; set; }    
    public int UserId { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public string Country { get; set; }

    public double WatchTimeHours { get; set; }
    public int MoviesWatched { get; set; }
    public int EpisodesWatched { get; set; }
    public double AverageRating { get; set; }
    public int ReviewsWritten { get; set; }

    public double Score { get; set; }
}
