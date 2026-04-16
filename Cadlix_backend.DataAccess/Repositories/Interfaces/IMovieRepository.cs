using Cadlix_backend.Domain.Entities.Movie;

namespace Cadlix_backend.DataAccess.Repositories.Interfaces;

public interface IMovieRepository
{
    List<MovieData> GetAll();
    MovieData? GetById(int id);
    MovieData Add(MovieData entity);
    MovieData? Update(MovieData entity);
    bool Delete(int id);
}
