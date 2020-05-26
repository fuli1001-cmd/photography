using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Application.Commands.CheckProcessed;
using Photography.Services.Order.API.Application.Commands.ConfirmShot;
using Photography.Services.Order.API.Application.Commands.SelectOriginal;
using Photography.Services.Order.API.Application.Commands.UploadOriginal;
using Photography.Services.Order.API.Application.Commands.UploadProcessed;
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
            var orders = await _orderQueries.GetOrdersAsync(new List<OrderStatus> { OrderStatus.WaitingForShooting });
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待上传原片和待选择原片订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadandselectoriginal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadOriginalOrdersAsync()
        {
            var status = new List<OrderStatus> { OrderStatus.WaitingForUploadOriginal, OrderStatus.WaitingForSelection };
            var orders = await _orderQueries.GetOrdersAsync(status);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        ///// <summary>
        ///// 获取待上传原片订单列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("waitingforuploadoriginal")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadOriginalOrdersAsync()
        //{
        //    var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForUploadOriginal);
        //    return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        //}

        ///// <summary>
        ///// 获取待选片订单列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("waitingforselection")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForSelectionOrdersAsync()
        //{
        //    var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForSelection);
        //    return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        //}

        /// <summary>
        /// 获取待出片和待验收订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadandcheckprocessed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadProcessedOrdersAsync()
        {
            var status = new List<OrderStatus> { OrderStatus.WaitingForUploadProcessed, OrderStatus.WaitingForCheck };
            var orders = await _orderQueries.GetOrdersAsync(status);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        ///// <summary>
        ///// 获取待出片订单列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("waitingforuploadprocessed")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForUploadProcessedOrdersAsync()
        //{
        //    var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForUploadProcessed);
        //    return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        //}

        ///// <summary>
        ///// 获取待验收订单列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("waitingforcheck")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetWaitingForCheckOrdersAsync()
        //{
        //    var orders = await _orderQueries.GetOrdersAsync(OrderStatus.WaitingForCheck);
        //    return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        //}

        /// <summary>
        /// 获取已完成订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("finished")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetFinishedOrdersAsync()
        {
            var status = new List<OrderStatus> { OrderStatus.Finished, OrderStatus.Canceled, OrderStatus.Rejected };
            var orders = await _orderQueries.GetOrdersAsync(status);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(orders));
        }

        /// <summary>
        /// 确认已拍片
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("confirmshot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ConfirmShotAsync([FromBody] ConfirmShotCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 上传原片
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("uploadoriginal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UploadOriginalAsync([FromBody] UploadOriginalCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 选择原片
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("selectoriginal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> SelectOriginalAsync([FromBody] SelectOriginalCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 上传精修片
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("uploadprocessed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UploadProcessedAsync([FromBody] UploadProcessedCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 确认精修片
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("checkprocessed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckProcessedAsync([FromBody] CheckProcessedCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
