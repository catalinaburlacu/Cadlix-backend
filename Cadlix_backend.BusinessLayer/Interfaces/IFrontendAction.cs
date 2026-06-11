using Cadlix_backend.Domain.DTOs.Frontend;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IFrontendAction
{
    HomePayloadDto GetHome();
    TrendingPayloadDto GetTrending(string? period = null, string? typeFilter = null);
    ExplorePayloadDto GetExplore();
    UserProfileDto? GetProfile(int userId);
    LeaderboardPayloadDto GetLeaderboardPage(int count = 100);
    ContentCardDto CreateContent(CreateContentDTO createDto);
}
