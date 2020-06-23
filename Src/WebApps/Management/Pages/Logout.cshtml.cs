using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Photography.WebApps.Management.Pages
{
    public class LogoutModel : PageModel
    {
        public async void OnGetAsync()
        {
            try
            {
                // Remove cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // Signout oidc
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
            catch { }
        }
    }
}
