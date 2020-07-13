using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Photography.WebApps.Management.Pages;
using Photography.WebApps.Management.Services;
using Photography.WebApps.Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.Data
{
    public class UserService
    {
        private readonly UserHttpService _userHttpService;
        private readonly ILogger<UserService> _logger;

        public PagedResponseWrapper<List<ViewModels.User>> PagedData { get; private set; }

        public UserService(UserHttpService userHttpService, ILogger<UserService> logger)
        {
            _userHttpService = userHttpService ?? throw new ArgumentNullException(nameof(userHttpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResponseWrapper<List<ViewModels.User>>> GetUsersAsync(int pageNumber, int pageSize)
        {
            try
            {
                PagedData = await _userHttpService.GetUsersAsync(pageNumber, pageSize);
                return PagedData;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUsersAsync failed, {@Exception}", ex);
                PagedData = new PagedResponseWrapper<List<ViewModels.User>> { Data = new List<ViewModels.User>(), PagingInfo = new PagingInfo() };
                return PagedData;
            }
        }

        public async Task<bool> UpdateUserAsync(ViewModels.User user)
        {
            try
            {
                return await _userHttpService.UpdateUserAsync(user);
            }
            catch(Exception ex)
            {
                _logger.LogError("UpdateUserAsync failed, {@Exception}", ex);
                return false;
            }
        }

        public async Task<bool> UpdateUserBackgroundAsync(ViewModels.User user)
        {
            try
            {
                return await _userHttpService.UpdateUserBackgroundAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateUserBackgroundAsync failed, {@Exception}", ex);
                return false;
            }
        }

        public async Task<bool> AuthRealNameAsync(ViewModels.User user, bool passed)
        {
            try
            {
                return await _userHttpService.AuthRealNameAsync(user, passed);
            }
            catch (Exception ex)
            {
                _logger.LogError("AuthRealNameAsync failed, {@Exception}", ex);
                return false;
            }
        }

        public async Task<bool> DisableUserAsync(ViewModels.User user, bool disabled)
        {
            try
            {
                var result = await _userHttpService.DisableUserAsync(user, disabled);
                
                if (result)
                    user.Disabled = disabled;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("DisableUserAsync failed, {@Exception}", ex);
                return false;
            }
        }
    }
}
