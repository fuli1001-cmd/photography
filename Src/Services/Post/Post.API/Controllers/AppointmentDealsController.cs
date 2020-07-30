using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.AcceptAppointmentDeal;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointTask;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointUser;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.CancelAppointmentDeal;
using Photography.Services.Post.API.Application.Commands.AppointmentDeal.RejectAppointmentDeal;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    /// <summary>
    /// 约拍交易控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class AppointmentDealsController : ControllerBase
    {
        private readonly ILogger<AppointmentDealsController> _logger;
        private readonly IMediator _mediator;
        private readonly IAppointmentDealQueries _appointmentDealQueries;

        public AppointmentDealsController(IMediator mediator, IAppointmentDealQueries appointmentDealQueries, ILogger<AppointmentDealsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _appointmentDealQueries = appointmentDealQueries ?? throw new ArgumentNullException(nameof(appointmentDealQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 约拍任务
        /// </summary>
        /// <param name="appointCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> AppointTaskAsync([FromBody] AppointTaskCommand appointCommand)
        {
            var result = await _mediator.Send(appointCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 约拍人
        /// </summary>
        /// <param name="appointCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> AppointUserAsync([FromBody] AppointUserCommand appointCommand)
        {
            var result = await _mediator.Send(appointCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取我发出的约拍交易
        /// </summary>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("sent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetSentAppointmentDealsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var deals = await _appointmentDealQueries.GetSentAppointmentDealsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(deals));
        }

        /// <summary>
        /// 获取我收到的约拍交易
        /// </summary>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("received")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetReceivedAppointmentDealsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var deals = await _appointmentDealQueries.GetReceivedAppointmentDealsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(deals));
        }

        /// <summary>
        /// 获取我发出的和收到的约拍交易数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SentAndReceivedAppointmentDealCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SentAndReceivedAppointmentDealCountViewModel>> GetSentAppointmentDealsCountAsync()
        {
            var result = await _appointmentDealQueries.GetSentAndReceivedAppointmentDealCountAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
