using AutoMapper;
using Cadlix_backend.Domain.DTOs.Content;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs.History;
using Cadlix_backend.Domain.DTOs.Lists;
using Cadlix_backend.Domain.Entities.History;
using Cadlix_backend.Domain.Entities.ListsOfUserFilms;
using Cadlix_backend.Domain.Entities.Movie;

namespace Cadlix_backend.BusinessLayer.Mapping;

public class MapProfile : Profile
{
    public MapProfile()
    {
        // History mappings
        CreateMap<HistoryData, HistoryDTO>();
        CreateMap<CreateHistoryDTO, HistoryData>();
        CreateMap<UpdateHistoryDTO, HistoryData>();

        // Lists mappings
        CreateMap<ListsData, ListDTO>();
        CreateMap<CreateListDTO, ListsData>();
        CreateMap<UpdateListDTO, ListsData>();

        // Content mappings
        CreateMap<MovieData, ContentDTO>()
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                src.Genres != null ? src.Genres.Select(g => g.Name).ToList() : null));
        CreateMap<CreateContentDTO, MovieData>()
            .ForMember(dest => dest.Genres, opt => opt.Ignore());
        CreateMap<UpdateContentDTO, MovieData>()
            .ForMember(dest => dest.Genres, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Frontend mappings
        CreateMap<ListsData, WatchListItemDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.FilmTitle))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.FilmStatus))
            .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.AddedAt))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.FilmScore ?? src.FilmRating));
        CreateMap<HistoryData, WatchHistoryItemDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.MovieTitle));
    }
}
