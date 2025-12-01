using System;
using Microsoft.AspNetCore.Http;

namespace SparWebCore.Helpers
{
    public static class UtilCompat
    {
        /// <summary>
        /// Compatibility shim for the old Util.IsRegistrationPopupAllowed(HttpRequestBase)
        /// Returns true when the user is not authenticated and the current URL is not a register page.
        /// </summary>
        public static bool IsRegistrationPopupAllowed(HttpRequest request)
        {
            if (request == null) return false;

            var user = request.HttpContext?.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated == true;

            var rawUrl = (request.Path + request.QueryString).ToString().ToLowerInvariant();

            return isAuthenticated == false && !rawUrl.Contains("/register");
        }
    }
}
