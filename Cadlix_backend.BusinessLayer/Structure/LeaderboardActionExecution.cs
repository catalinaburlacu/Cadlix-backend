using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Services;
using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Structure;

public class LeaderboardActionExecution: LeaderboardActions, ILeaderboardAction
{
    public IEnumerable<LeaderboardEntryDto> GetTopUsers(int count = 100)
    {
        return GetTopUsersAsync(count).GetAwaiter().GetResult();
    }
    public LeaderboardEntryDto? GetUserRank(int userId)
    {
        return GetUserRankAsync(userId).GetAwaiter().GetResult();
    }
    public double CalculateScore(int userId)
    {
        return CalculateScoreAsync(userId).GetAwaiter().GetResult();
    }
}
