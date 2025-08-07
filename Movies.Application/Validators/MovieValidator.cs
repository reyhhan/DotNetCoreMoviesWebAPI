using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;
        public MovieValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

            RuleFor(x=>x.Id).NotEmpty().WithMessage("Movie ID cannot be empty.");

            RuleFor(x => x.Genres).NotEmpty();

            RuleFor(x => x.Title).NotEmpty();

            RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("This movie already exists");
        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(slug, token);
            if(existingMovie is not null)
            {
                return existingMovie.Id == movie.Id;
            }
            return existingMovie is null;
        }
    }
}
