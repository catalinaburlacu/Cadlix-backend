using System;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Structure;

namespace Cadlix_backend.BusinessLayer;

public class BusinessLogic
{
    public IUserAction User()
    {
        return new UserActionExecution();
    }

    public ILeaderboardAction Leaderboard()
    {
        return new LeaderboardActionExecution();
    }

    public ISubscriptionAction Subscription()
    {
        return new SubscriptionActionExecution();
    }
}
