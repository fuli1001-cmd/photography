using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Appointment.DeleteAppointment;
using Photography.Services.Post.API.Application.Commands.Appointment.PublishAppointment;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    /// <summary>
    /// 约拍控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IMediator _mediator;
        private readonly IAppointmentQueries _appointmentQueries;

        public AppointmentsController(IMediator mediator, IAppointmentQueries appointmentQueries, ILogger<AppointmentsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _appointmentQueries = appointmentQueries ?? throw new ArgumentNullException(nameof(appointmentQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 发布约拍
        /// </summary>
        /// <param name="publishPostCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AppointmentViewModel>> PublishAsync([FromBody] PublishAppointmentCommand publishAppointmentCommand)
        {
            var post = await _mediator.Send(publishAppointmentCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
        }

        /// <summary>
        /// 删除约拍
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeletePostAsync([FromBody] DeleteAppointmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取约拍广场的约拍列表
        /// </summary>
        /// <param name="payerType">付款方类型：0 - 互勉，1 - 发布方付费，2 - 约拍方付费，3 - 协商</param>
        /// <param name="appointmentedUserType">约拍对象类型：0 - 摄影师，1 - 模特</param>
        /// <param name="appointmentSeconds">约拍日期时间戳（距unix epoch的秒数）</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetAppointmentsAsync(
            [FromQuery(Name = "payertype")] PayerType? payerType, 
            [FromQuery(Name = "appointmentedUserType")] AppointmentedUserType? appointmentedUserType, 
            [FromQuery(Name = "appointmentseconds")] double? appointmentSeconds, 
            [FromQuery] PagingParameters pagingParameters)
        {
            var appointments = await _appointmentQueries.GetAppointmentsAsync(payerType, appointmentedUserType, appointmentSeconds, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(appointments));
        }

        /// <summary>
        /// 获取我发布的约拍
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetMyAppointmentsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var appointments = await _appointmentQueries.GetMyAppointmentsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(appointments));
        }
    }
}
