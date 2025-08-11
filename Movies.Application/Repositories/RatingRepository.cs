﻿using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public RatingRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> DeleteRatinAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                delete from ratings
                where movieid = @movieId
                and userid = @userId
                """, new { movieId, userId }, cancellationToken: token));
   
            return result > 0;
        }

        public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                select round(avg(ratings), 1) as rating
                from ratings r
                where movieid= @movieid
                """, new { movieId }, cancellationToken: token));
        }

        public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
                select round(avg(ratings), 1) as rating,
                  (select ratings
                  from ratings 
                  where movieid=@movieId 
                    and userid= @userId
                  limit 1)
                from ratings
                where movieid= @movieId
                """, new { movieId, userId }, cancellationToken: token));
        }

        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                select r.ratings, r.movieid, m.slug
                from ratings r
                inner join movies m on r.movieid = m.id
                where userid = @userId
                """, new { userId }, cancellationToken: token));
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                insert into ratings (movieid, userid, ratings)
                values (@movieId, @userId, @rating)
                on conflict (movieid, userid) do update
                set ratings = @rating
                """, new { movieId, userId, rating }, cancellationToken: token));
            return result > 0;

        }
    }
}
