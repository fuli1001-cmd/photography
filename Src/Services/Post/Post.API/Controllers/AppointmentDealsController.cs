using Arise.DDD.API;
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
        public async Task<ActionResult<bool>> AppointTaskAsync([FromBody] AppointTaskCommand appointCommand)
        {
            var result = await _mediator.Send(appointCommand);
            return StatusCode((int)HttpStatusCode.Created, ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 约拍人
        /// </summary>
        /// <param name="appointCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AppointUserAsync([FromBody] AppointUserCommand appointCommand)
        {
            var result = await _mediator.Send(appointCommand);
            return StatusCode((int)HttpStatusCode.Created, ResponseWrapper.CreateOkResponseWrapper(true));
        }

        /// <summary>
        /// 取消我发出的约拍交易
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CancelAppointmentDealAsync([FromBody] CancelAppointmentDealCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 确认别人发出的约拍交易
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AcceptAppointmentDealAsync([FromBody] AcceptAppointmentDealCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 拒绝别人发出的约拍交易
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> RejectAppointmentDealAsync([FromBody] RejectAppointmentDealCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取我发出的约拍交易
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentViewModel>>> GetSentAppointmentDealsAsync()
        {
            var deals = await _appointmentDealQueries.GetSentAppointmentDealsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(deals));
        }

        /// <summary>
        /// 获取我收到的约拍交易
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("received")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentViewModel>>> GetReceivedAppointmentDealsAsync()
        {
            var deals = await _appointmentDealQueries.GetReceivedAppointmentDealsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(deals));
        }
    }
}
