using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Application.Commands.Group.ChangeGroupOwner;
using Photography.Services.User.API.Application.Commands.Group.CreateGroup;
using Photography.Services.User.API.Application.Commands.Group.DeleteGroup;
using Photography.Services.User.API.Application.Commands.Group.EnableModifyMember;
using Photography.Services.User.API.Application.Commands.Group.ModifyGroupMembers;
using Photography.Services.User.API.Application.Commands.Group.MuteGroup;
using Photography.Services.User.API.Application.Commands.Group.QuitGroup;
using Photography.Services.User.API.Application.Commands.Group.UpdateGroup;
using Photography.Services.User.API.Application.Commands.Group.UpdateGroupAvatar;
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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupQueries _groupQueries;
        private readonly ILogger<GroupsController> _logger;
        private readonly IMediator _mediator;

        public GroupsController(IMediator mediator, 
            IGroupQueries groupQueries,
            ILogger<GroupsController> logger)
        {
            _groupQueries = groupQueries ?? throw new ArgumentNullException(nameof(groupQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 建群
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> CreateGroupAsync([FromBody] CreateGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 编辑群
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> UpdateGroupAsync([FromBody] UpdateGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 编辑群图片
        /// 该接口用于此情况：
        /// 群已打开允许群成员修改群成员，在群成员（非群主）修改了群成员后，客户端会根据最新成员列表更新群头像，
        /// 此时不能调用编辑群的接口，因为那个接口检查了操作者是否是群主，并且会发通知，而本接口情况无需发送通知
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> UpdateGroupAvatarAsync([FromBody] UpdateGroupAvatarCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 解散群
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> DeleteGroupAsync([FromBody] DeleteGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 更改群主
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("setowner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> ChangeGroupOwnerAsync([FromBody] ChangeGroupOwnerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 设置群免打扰
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("mute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> MuteGroupAsync([FromBody] MuteGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 设置允许群成员添加/删除群成员
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("enablemodifymember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> MuteGroupAsync([FromBody] EnableModifyMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 增加/删除群成员
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> ModifyGroupMembersAsync([FromBody] ModifyGroupMembersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("quit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> QuitGroupAsync([FromBody] QuitGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取群信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("group")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetGroupAsync([FromQuery(Name = "groupId")] Guid? groupId, [FromQuery(Name = "oldGroupId")] int? oldGroupId)
        {
            var result = await _groupQueries.GetGroupAsync(groupId, oldGroupId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 用户的群列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetGroupsAsync()
        {
            var result = await _groupQueries.GetGroupsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
