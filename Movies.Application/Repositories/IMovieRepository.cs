using Movies.Application.Models;
using System;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
       Task<bool> CreateAsync(Movie movie, CancellationToken token);
       Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default);
       Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default);

       Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);

       Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);

       Task<bool> DeleteByIdAsync(Guid id, CancellationToken token);

        Task<bool> ExistsById(Guid id, CancellationToken token);
    }
}
