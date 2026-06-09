using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Structure;

public class LeaderboardActionExecution: LeaderboardActions, ILeaderboardAction
{
    public IEnumerable<LeaderboardEntryDto> GetTopUsers(int count = 100)
    {
        return GetTopUsers(count);
    }
    public LeaderboardEntryDto? GetUserRank(int userId)
    {
        return GetUserRank(userId);
    }
    public double CalculateScore(int userId)
    {
        return CalculateScore(userId);
    }
}
