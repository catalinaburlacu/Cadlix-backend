using System;
using AutoMapper;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Mapping;
using Cadlix_backend.BusinessLayer.Core;
using Cadlix_backend.BusinessLayer.Structure;

namespace Cadlix_backend.BusinessLayer;

public class BusinessLogic
{
    private readonly IMapper _mapper;

    public BusinessLogic()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MapProfile>();
        });
        _mapper = config.CreateMapper();
    }

    public IUserAction User()
    {
        return new UserActions();
    }

    public ILeaderboardAction Leaderboard()
    {
        return new LeaderboardActions();
    }

    public ISubscriptionAction Subscription()
    {
        return new SubscriptionActions();
    }

    public IFrontendAction Frontend()
    {
        return new FrontendActions(_mapper);
    }

    public IHistoryAction History()
    {
        return new HistoryActions(_mapper);
    }

    public IListsAction Lists()
    {
        return new ListsActions(_mapper);
    }

    public IContentAction Content()
    {
        return new ContentActions(_mapper);
    }

    public IMovieAction MovieAction()
    {
        return new MovieActionExecution();
    }
}
