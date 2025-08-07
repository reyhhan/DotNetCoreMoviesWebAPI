using Movies.Application.Models;
using System;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
       Task<bool> CreateAsync(Movie movie, CancellationToken token);
       Task<Movie?> GetByIdAsync(Guid id, CancellationToken token);
       Task<Movie?> GetBySlugAsync(string slug, CancellationToken token);

       Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token);

       Task<bool> UpdateAsync(Movie movie, CancellationToken token);

       Task<bool> DeleteByIdAsync(Guid id, CancellationToken token);

        Task<bool> ExistsById(Guid id, CancellationToken token);
    }
}
