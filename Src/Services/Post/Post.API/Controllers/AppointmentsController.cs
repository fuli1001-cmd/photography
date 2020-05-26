using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        /// 获取约拍广场的约拍列表
        /// </summary>
        /// <param name="payerType">付款方类型</param>
        /// <param name="appointmentSeconds">约拍日期时间戳（距unix epoch的秒数）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentViewModel>>> GetAppointmentsAsync([FromQuery(Name = "payertype")] PayerType? payerType, [FromQuery(Name = "appointmentseconds")] double? appointmentSeconds)
        {
            var appointments = await _appointmentQueries.GetAppointmentsAsync(payerType, appointmentSeconds);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(appointments));
        }

        /// <summary>
        /// 获取我发布的约拍
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentViewModel>>> GetMyAppointmentsAsync()
        {
            var appointments = await _appointmentQueries.GetMyAppointmentsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(appointments));
        }
    }
}
