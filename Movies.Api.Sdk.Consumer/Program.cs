using Movies.Api.Sdk;
using System.Text.Json;
using Refit;
using Movies.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk.Consumer;

//var moviesApi = RestService.For<IMoviesApi>("https://localhost:7032");

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (request, cancellationToken) =>
        {
            var provider = s.GetRequiredService<AuthTokenProvider>();
            return await provider.GetTokenAsync();

        }
    })
    .ConfigureHttpClient(x =>
        x.BaseAddress = new Uri("https://localhost:7032"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

//Get Movie
var movie = await moviesApi.GetMovieAsync("nickthegreeki-2023");

//Get All Movies
var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 3
};

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action" }
});

await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action", "Adventure" }
});

await moviesApi.DeleteMovieAsync(newMovie.Id);

var movies = await moviesApi.GetMoviesAsync(request);

Console.WriteLine(JsonSerializer.Serialize(movies));