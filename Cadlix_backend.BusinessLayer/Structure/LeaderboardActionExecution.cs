using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Structure;

public class LeaderboardActionExecution: LeaderboardActions, ILeaderboardAction
{
    public new IEnumerable<LeaderboardEntryDto> GetTopUsers(int count = 100)
    {
        return base.GetTopUsers(count);
    }
    public new LeaderboardEntryDto? GetUserRank(int userId)
    {
        return base.GetUserRank(userId);
    }
    public new double CalculateScore(int userId)
    {
        return base.CalculateScore(userId);
    }
}
