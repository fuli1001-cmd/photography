﻿using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Arise.DDD.Infrastructure.Data;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<Domain.AggregatesModel.UserAggregate.User, UserContext>, IUserRepository
    {
        public UserRepository(UserContext context) : base(context)
        {

        }

        public async Task<Domain.AggregatesModel.UserAggregate.User> GetByNicknameAsync(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                return null;

            var users = await _context.Users.Where(u => !string.IsNullOrEmpty(u.Nickname) && u.Nickname.ToLower() == nickname.ToLower()).ToListAsync();
            if (users.Count > 0)
                return users[0];
            else
                return null;
        }

        public async Task<Domain.AggregatesModel.UserAggregate.User> GetByUserNameAsync(string userName)
        {
            var users = await _context.Users.Where(u => u.UserName == userName).ToListAsync();
            if (users.Count > 0)
                return users[0];
            else
                return null;
        }

        public async Task<Guid?> GetUserIdByCodeAsync(string code)
        {
            if (code == null)
                return null;

            var userIds = await (from u in _context.Users
                                 where u.Code.ToLower() == code.ToLower()
                                 select u.Id).ToListAsync();

            if (userIds.Count > 0)
                return userIds[0];
            else
                return null;
        }

        public async Task<IEnumerable<Domain.AggregatesModel.UserAggregate.User>> GetUsersAsync(IEnumerable<Guid> userIds)
        {
            return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        }
    }
}
