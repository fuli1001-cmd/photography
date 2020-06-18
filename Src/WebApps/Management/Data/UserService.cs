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

        public List<ViewModels.User> Users { get; private set; }

        public UserService(UserHttpService userHttpService, ILogger<UserService> logger)
        {
            _userHttpService = userHttpService ?? throw new ArgumentNullException(nameof(userHttpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Photography.WebApps.Management.ViewModels.User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            Users = await _userHttpService.GetUsersAsync(pageNumber, pageSize);
            return Users;
        }

        public async Task<bool> UpdateUserAsync(Photography.WebApps.Management.ViewModels.User user)
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
    }
}
