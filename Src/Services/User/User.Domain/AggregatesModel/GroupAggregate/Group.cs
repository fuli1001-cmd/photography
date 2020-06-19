using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.GroupAggregate
{
    public class Group : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Notice { get; private set; }
        public string Avatar { get; private set; }
        public bool ModifyMemberEnabled { get; private set; }
        public double CreatedTime { get; private set; }

        // BackwardCompatibility: ChatServer needed Property
        public int ChatServerGroupId { get; private set; }

        // 群主
        public Guid OwnerId { get; private set; }
        public UserAggregate.User Owner { get; private set; }

        // 与user的多对多关系
        private readonly List<GroupUser> _groupUsers = null;
        public IReadOnlyCollection<GroupUser> GroupUsers => _groupUsers;

        public Group()
        {
            ModifyMemberEnabled = false;
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            _groupUsers = new List<GroupUser>();
        }

        public Group(string name, string avatar, Guid ownerId, List<Guid> memberIds) : this()
        {
            Name = name;
            Avatar = avatar;
            OwnerId = ownerId;

            if (memberIds == null)
                memberIds = new List<Guid>();

            // 确保群主也在群中
            if (!memberIds.Contains(ownerId))
                memberIds.Add(ownerId);

            AddMembers(memberIds);
        }

        public void AddMembers(List<Guid> memberIds)
        {
            memberIds.ForEach(memberId => _groupUsers.Add(new GroupUser(memberId)));
        }

        public void Update(string name, string notice, string avatar)
        {
            Name = name;
            Notice = notice;
            Avatar = avatar;
        }

        public void Delete()
        {
            AddDeletedGroupDomainEvent();
        }

        public void EnableAddMember(Guid ownerId)
        {
            if (OwnerId != ownerId)
                throw new ClientException("操作失败", new List<string> { $"Group {Id} does not belong to user {ownerId}" });

            ModifyMemberEnabled = true;
        }

        public void DisableAddMember(Guid ownerId)
        {
            if (OwnerId != ownerId)
                throw new ClientException("操作失败", new List<string> { $"Group {Id} does not belong to user {ownerId}" });

            ModifyMemberEnabled = false;
        }

        public void ChangeOwner(Guid oldOwnerId, Guid newOwnerId)
        {
            // 检查原来的群主是当前用户，并且现在的群主是群成员
            if (OwnerId != oldOwnerId)
                throw new ClientException("操作失败", new List<string> { $"Group {Id} does not belong to user {oldOwnerId}" });

            if (!GroupUsers.Any(gu => gu.UserId == newOwnerId))
                throw new ClientException("操作失败", new List<string> { $"New owner {newOwnerId} does not belong to group {Id}" });

            OwnerId = newOwnerId;
        }

        private void AddDeletedGroupDomainEvent()
        {
            var deletedGroupDomainEvent = new DeletedGroupDomainEvent(Id);
            AddDomainEvent(deletedGroupDomainEvent);
        }
    }
}
