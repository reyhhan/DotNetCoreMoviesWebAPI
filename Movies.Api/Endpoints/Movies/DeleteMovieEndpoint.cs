using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies
{
    public static class DeleteMovieEndpoint
    {
        public const string Name = "DeleteMovie";

        public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.Update, async
                (Guid id, IMovieService movieService, HttpContext context,
                IOutputCacheStore outputCachedStore, CancellationToken token) =>
            {
                bool isDeleted = await movieService.DeleteByIdAsync(id, token);
                if (!isDeleted)
                {
                    return Results.NotFound();
                }
                // Evict the cache for movies after deletion
                await outputCachedStore.EvictByTagAsync("movies", token);
                return TypedResults.Ok();

            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.AdminUserPolicyName);

            return app;
        }
    }
}
