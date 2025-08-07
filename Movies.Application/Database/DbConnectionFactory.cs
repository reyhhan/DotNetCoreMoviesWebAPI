﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync (CancellationToken token = default);
    }

    public class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connection;
        public NpgsqlConnectionFactory(string connection)
        {
            _connection = connection;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
        {
            var connection = new NpgsqlConnection(_connection);
            await connection.OpenAsync(token);
            return connection;
        }
    }
}
