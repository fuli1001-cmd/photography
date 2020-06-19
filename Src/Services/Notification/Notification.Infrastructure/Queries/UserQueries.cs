using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Query.Interfaces;
using Photography.Services.Notification.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Photography.Services.Notification.Infrastructure.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly NotificationContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserQueries> _logger;

        public UserQueries(NotificationContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<UserQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PushSettingsViewModel> GetPushSettingsAsync()
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
            return await (from u in _dbContext.Users
                          where u.Id == myId
                          select new PushSettingsViewModel
                          {
                              PushLikeEvent = u.PushLikeEvent,
                              PushReplyEvent = u.PushReplyEvent,
                              PushForwardEvent = u.PushForwardEvent,
                              PushShareEvent = u.PushShareEvent,
                              PushFollowEvent = u.PushFollowEvent
                          })
                          .SingleOrDefaultAsync();
        }
    }
}
