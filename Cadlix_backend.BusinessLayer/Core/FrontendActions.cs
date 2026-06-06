using AutoMapper;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs.History;
using Cadlix_backend.Domain.Entities.History;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.Movie;
using Cadlix_backend.Domain.Entities.Subscription;
using Cadlix_backend.Domain.Entities.User;
using UserEntity = Cadlix_backend.Domain.Entities.User.UserData;

namespace Cadlix_backend.BusinessLayer.Core;

public partial class FrontendActions : IFrontendAction
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FrontendActions(IMapper mapper)
    {
        _context = new AppDbContext();
        _mapper = mapper;
    }
}
