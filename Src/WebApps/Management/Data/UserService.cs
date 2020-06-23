﻿using Microsoft.AspNetCore.Mvc.RazorPages;
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
                PagedData.Data.ForEach(u => _logger.LogInformation("{Nickname}, {Avatar}", u.Nickname, u.Avatar));
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
    }
}
