using Movies.Application.Models;
using System;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
       Task<bool> CreateAsync(Movie movie);
       Task<Movie?> GetByIdAsync(Guid Id);
       Task<IEnumerable<Movie>> GetAllAsync();

       Task<bool> UpdateAsync(Movie movie);

       Task<bool> DeleteByIdAsync(Guid Id);
    }
}
