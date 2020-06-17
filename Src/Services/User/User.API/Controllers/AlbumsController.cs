using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Application.Commands.Album.CreateAlbum;
using Photography.Services.User.API.Application.Commands.Album.DeleteAlbum;
using Photography.Services.User.API.Application.Commands.Album.UpdateAlbum;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumQueries _albumQueries;
        private readonly ILogger<AlbumsController> _logger;
        private readonly IMediator _mediator;

        public AlbumsController(IMediator mediator, IAlbumQueries albumQueries, ILogger<AlbumsController> logger)
        {
            _albumQueries = albumQueries ?? throw new ArgumentNullException(nameof(albumQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取用户的所有相册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetAlbumsAsync()
        {
            var albums = await _albumQueries.GetAlbumsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(albums));
        }

        /// <summary>
        /// 分页获取用户的相册
        /// </summary>
        /// <param name="orderBy">排序字段：CreatedTime或UpdatedTime</param>
        /// <param name="asc">true：顺序，false：倒序</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{orderBy}/{asc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetPagedAlbumsAsync(string orderBy, bool asc, [FromQuery] PagingParameters pagingParameters)
        {
            var albums = await _albumQueries.GetAlbumsAsync(pagingParameters, orderBy, asc);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(albums));
        }

        /// <summary>
        /// 分页获取相册内的照片
        /// </summary>
        /// <param name="albumId">相册id</param>
        /// <param name="orderBy">排序字段：CreatedTime或UpdatedTime</param>
        /// <param name="asc">true：顺序，false：倒序</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{albumId}/{orderBy}/{asc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetPagedAlbumPhotosAsync(Guid albumId, string orderBy, bool asc, [FromQuery] PagingParameters pagingParameters)
        {
            var photos = await _albumQueries.GetAlbumPhotosAsync(albumId, pagingParameters, orderBy, asc);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(photos));
        }

        /// <summary>
        /// 创建相册
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> CreateAlbumAsync([FromBody] CreateAlbumCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 编辑相册
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> UpdateAlbumAsync([FromBody] UpdateAlbumCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 删除相册
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> DeleteAlbumAsync([FromBody] DeleteAlbumCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
