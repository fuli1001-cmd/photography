using Photography.Services.Post.API.Application.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Queries
{
    public interface IPostQueries
    {
        Task<IEnumerable<PostViewModel>> GetGamesAsync();
    }
}
