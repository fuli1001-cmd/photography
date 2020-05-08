using Dapper;
using Photography.Services.Post.API.Application.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Queries
{
    public class PostQueries : IPostQueries
    {
        private string _connectionString = string.Empty;

        public PostQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<IEnumerable<PostViewModel>> GetGamesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PostViewModel>(@"");
            }
        }
    }
}
