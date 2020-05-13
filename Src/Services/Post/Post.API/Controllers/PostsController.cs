using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostQueries _postQueries;
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator, IPostQueries postQueries, ILogger<PostsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("hot")]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetHotPostsAsync()
        {
            _logger.LogInformation("-----UserId: {UserId}-----", User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _logger.LogInformation("-----UserName: {UserName}-----", User.FindFirst(ClaimTypes.Name)?.Value);
            _logger.LogInformation("-----UserName: {UserName}-----", User.Identity.Name);
            _logger.LogInformation("-----Authed: {Authed}-----", User.Identity.IsAuthenticated);
            _logger.LogInformation("-----AuthType: {AuthType}-----", User.Identity.AuthenticationType);
            var posts = await _postQueries.GetPostsAsync(PostType.Hot);
            return Ok(posts);
        }

        [HttpGet]
        [Route("followed")]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetFollowedPostsAsync()
        {
            var posts = await _postQueries.GetPostsAsync(PostType.Followed);
            return Ok(posts);
        }
    }
}
