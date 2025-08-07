using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, CancellationToken token);
        Task<Movie?> GetByIdAsync(Guid id, CancellationToken token);
        Task<Movie?> GetBySlugAsync(string slug, CancellationToken token);

        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token);

        Task<Movie?> UpdateAsync(Movie movie, CancellationToken token);

        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token);

        Task<bool> ExistsById(Guid id, CancellationToken token);
    }
}
