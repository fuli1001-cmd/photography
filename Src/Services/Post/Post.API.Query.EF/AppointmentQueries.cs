using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class AppointmentQueries : IAppointmentQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentQueries> _logger;

        public AppointmentQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<AppointmentQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<AppointmentViewModel>> GetAppointmentsAsync(PayerType? payerType, double? appointmentSeconds)
        {
            var posts = _postContext.Posts.Where(p => p.PostType == PostType.Appointment);
            
            if (payerType != null)
                posts = posts.Where(p => p.PayerType == payerType);

            if (appointmentSeconds != null)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
                var date = DateTime.UnixEpoch.AddSeconds(appointmentSeconds.Value);
                var startSeconds = (new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0) - epoch).TotalSeconds;
                var endSeconds = (new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999) - epoch).TotalSeconds;
                posts = posts.Where(p => p.AppointedTime != null && p.AppointedTime.Value >= startSeconds && p.AppointedTime.Value <= endSeconds);
            }

            posts = GetPostsWithNavigationPropertiesAsync(posts);
            var appointments = _mapper.Map<List<AppointmentViewModel>>(await posts.OrderByDescending(p => p.Timestamp).ToListAsync());
            appointments.ForEach(a => a.SetAttachmentProperties(_logger));
            return appointments;
        }

        public Task<List<AppointmentViewModel>> GetReceivedAppointmentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<AppointmentViewModel>> GetSentAppointmentsAsync()
        {
            throw new NotImplementedException();
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetPostsWithNavigationPropertiesAsync(IQueryable<Domain.AggregatesModel.PostAggregate.Post> query)
        {
            return query
                .Include(p => p.User)
                .Include(p => p.PostAttachments);
        }
    }
}
