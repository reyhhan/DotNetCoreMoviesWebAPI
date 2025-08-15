using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies
{
    public static class GetMovieEndpoint
    {
        public const string Name = "GetMovie";

        public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Movies.GetById, async (string idOrSlug,
                IMovieService movieService, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, token)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, token);
                if (movie == null)
                {
                    return Results.NotFound();
                }
                return TypedResults.Ok(movie.MapToResponse());
            }).WithName($"{Name}V1")
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);

            app.MapGet(ApiEndpoints.Movies.GetById, async (string idOrSlug,
               IMovieService movieService, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, token)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, token);
                if (movie == null)
                {
                    return Results.NotFound();
                }
                return TypedResults.Ok(movie.MapToResponse());
            }).WithName($"{Name}V2")
           .Produces<MovieResponse>(StatusCodes.Status200OK)
           .WithApiVersionSet(ApiVersioning.VersionSet)
           .HasApiVersion(2.0);

            return app;
        }
    }
}
