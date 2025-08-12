using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(new CommandDefinition("""                
                INSERT INTO movies (id, slug, title, yearOfRelease) VALUES (@Id, @Slug, @Title, @YearOfRelease)               
                """, movie, cancellationToken: token));

            if (result > 0)
            {
                foreach(var genre in movie.Genres)
                {
                    var genreResult = await connection.ExecuteAsync(new CommandDefinition("""
                        INSERT INTO genres (movieId, name) VALUES (@MovieId, @Name)
                        """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
                   
                }
            }
            transaction.Commit();
            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(Guid Id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("""
                delete from genres where movieid= @id
                """, new { id = Id }, cancellationToken: token));

            var result  = await connection.ExecuteAsync(new CommandDefinition("""
                delete from movies where id= @id
                """, new { id = Id }, cancellationToken: token));

            transaction.Commit();

            return result > 0;  

        }

        public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                select count(1) from movies where id = @id
                """, new { id }, cancellationToken: token));
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var result = await connection.QueryAsync(new CommandDefinition("""
                select m.*, 
                    string_agg(distinct g.name, ',') as genres,
                    round(avg(r.ratings), 1) as rating,
                    myr.ratings as userrating
                FROM movies m 
                LEFT JOIN genres g on m.id = g.movieid
                LEFT JOIN ratings r on m.id = r.movieid
                LEFT JOIN ratings myr on m.id = myr.movieid 
                AND myr.userid = @userid
                WHERE (@title is null or LOWER(m.title) like ('%' || @title || '%'))
                AND (@yearofrelease is null or m.yearOfRelease = @yearofrelease)
                group by id, userrating
                """, new 
                { 
                  userid = options.UserId,
                  title = options.Title?.ToLower(),
                  yearofrelease = options.YearOfRelease
                },
                cancellationToken: token));
         
            return result.Select(x => new Movie
            {
                Id = (Guid)x.id,
                Title = (string)x.title,
                YearOfRelease = Convert.ToInt32(x.yearofrelease),
                Rating = x.rating !=null ? (float)x.rating  : 0,
                UserRating = Convert.ToInt32(x.rating ?? 0),
                Genres = x.genres is string genresStr
                    ? genresStr.Split(',').Select(g => g.Trim()).ToList()
                    : new List<string>()
            });
        }

        public async Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token =default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition("""
                    SELECT m.* , round(avg(r.ratings), 1) as rating, myr.ratings as userrating
                    FROM movies m
                    LEFT JOIN ratings r on m.id = r.movieid
                    LEFT JOIN ratings myr on m.id = myr.movieid 
                    AND myr.userid = @userid
                    WHERE id = @id
                    GROUP BY id, userrating
                    """, new { id, userid }, cancellationToken: token));

            if(movie == null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
                    SELECT name FROM genres WHERE movieid= @id
                    """, new { id }, cancellationToken: token));

            foreach(var genre in genres)
            {
                movie.Genres.Add(genre);
            }

            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default) 
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition("""
                    SELECT m.* , round(avg(r.ratings), 1) as ratings, myr.rating as userrating
                    FROM movies m
                    LEFT JOIN ratings r on m.id = r.movieid
                    LEFT JOIN ratings myr on m.id = myr.movieid 
                    AND myr.userid = @userid
                    WHERE slug = @slug
                    GROUP BY id, userrating
                    """, new { slug, userid }, cancellationToken: token));

            if (movie == null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
                    SELECT name FROM genres WHERE movieid= @id
                    """, new { id= movie.Id }, cancellationToken: token));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            return movie;

        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
           using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

           using var transaction = connection.BeginTransaction();

           await connection.ExecuteAsync(new CommandDefinition("""
                delete from genres where movieid= @id
                """, new { id = movie.Id }, cancellationToken: token));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                        INSERT INTO genres (movieId, name) VALUES (@MovieId, @Name)
                        """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                 update movies set slug = @Slug, title = @Title, yearOfRelease = @YearOfRelease
                 where id = @Id
                 """, movie, cancellationToken: token));

            transaction.Commit();

            return result > 0;
        }
    }
}
