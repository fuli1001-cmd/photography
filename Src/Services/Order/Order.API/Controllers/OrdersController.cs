using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Application.Commands.AcceptOrder;
using Photography.Services.Order.API.Application.Commands.CancelOrder;
using Photography.Services.Order.API.Application.Commands.CheckProcessed;
using Photography.Services.Order.API.Application.Commands.ConfirmShot;
using Photography.Services.Order.API.Application.Commands.RejectOrder;
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
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetOrderAsync(Guid orderId)
        {
            var order = await _orderQueries.GetOrderAsync(orderId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(order));
        }

        /// <summary>
        /// 根据约拍交易id获取订单详情
        /// </summary>
        /// <param name="dealId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("deal/{dealId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetOrderByDealIdAsync(Guid dealId)
        {
            var order = await _orderQueries.GetOrderByDealIdAsync(dealId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(order));
        }

        /// <summary>
        /// 获取待拍片阶段订单列表
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforshooting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetWaitingForShootingOrdersAsync([FromQuery] PagingParameters pagingParameters)
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStage.Shooting, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(orders));
        }

        /// <summary>
        /// 获取待选片阶段的订单列表
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadandselectoriginal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetWaitingForUploadOriginalOrdersAsync([FromQuery] PagingParameters pagingParameters)
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStage.Selection, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(orders));
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
        /// 获取待出片阶段订单列表
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("waitingforuploadandcheckprocessed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetWaitingForUploadProcessedOrdersAsync([FromQuery] PagingParameters pagingParameters)
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStage.Production, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(orders));
        }

        /// <summary>
        /// 获取订单各阶段数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StageOrderCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StageOrderCountViewModel>> GetStageOrderCountAsync()
        {
            var stageOrderCount = await _orderQueries.GetStageOrderCountAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(stageOrderCount));
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
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("finished")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetFinishedOrdersAsync([FromQuery] PagingParameters pagingParameters)
        {
            var orders = await _orderQueries.GetOrdersAsync(OrderStage.Finished, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(orders));
        }

        /// <summary>
        /// 确认约拍产生的订单
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderViewModel>> AcceptOrderAsync([FromBody] AcceptOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 取消约拍产生的订单
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderViewModel>> CancelOrderAsync([FromBody] CancelOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 拒绝约拍产生的订单
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderViewModel>> RejectOrderAsync([FromBody] RejectOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 确认已拍片
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("confirmshot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderViewModel>> ConfirmShotAsync([FromBody] ConfirmShotCommand command)
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
        public async Task<ActionResult<OrderViewModel>> UploadOriginalAsync([FromBody] UploadOriginalCommand command)
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
        public async Task<ActionResult<OrderViewModel>> SelectOriginalAsync([FromBody] SelectOriginalCommand command)
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
        public async Task<ActionResult<OrderViewModel>> UploadProcessedAsync([FromBody] UploadProcessedCommand command)
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
