using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Application.Commands.ConfirmShot;
using Photography.Services.Order.API.Query.Interfaces;
using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Controllers
{
    /// <summary>
    /// 订单控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderQueries _orderQueries;
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries, ILogger<OrdersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取待拍片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforshooting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForShootingOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForShooting);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待上传原片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadoriginal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadOriginalOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForUploadOriginal);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待选片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforselection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForSelectionOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForSelection);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待出片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadprocessed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadProcessedOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForUploadProcessed);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待出片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforcheck")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForCheckOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForCheck);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取已完成订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("finished")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetFinishedOrdersAsync()
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStatus.Finished);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 确认已拍片
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("shot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> GetFinishedOrdersAsync([FromBody] ConfirmShotCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
