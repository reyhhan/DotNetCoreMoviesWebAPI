using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _validator;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

        public MovieService(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<Movie> validator, IValidator<GetAllMoviesOptions> optionsValidator)
        {
            _movieRepository = movieRepository;
            _ratingRepository = ratingRepository;
            _validator = validator;
            _optionsValidator = optionsValidator;
        }
        public async Task<bool> CreateAsync(Movie movie, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
            return await _movieRepository.CreateAsync(movie, token);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token)
        {
          return _movieRepository.DeleteByIdAsync(id, token);
        }

        public Task<bool> ExistsById(Guid id,CancellationToken token )
        {
            return _movieRepository.ExistsById(id, token);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);

            return await _movieRepository.GetAllAsync(options, token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token= default)
        {
           return _movieRepository.GetByIdAsync(id, userid, token);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token =default)
        {
            return _movieRepository.GetBySlugAsync(slug, userid, token );
        } 

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userid = default, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);

            var movieExists = await _movieRepository.ExistsById(movie.Id, token);

            if (!movieExists)
            {
                return null;
            }
            await _movieRepository.UpdateAsync(movie, token);

            if (!userid.HasValue)
            {
                var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
                movie.Rating = rating;
                return movie;
            }

            var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userid.Value, token);
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;

            return movie;
        }
    }
}
