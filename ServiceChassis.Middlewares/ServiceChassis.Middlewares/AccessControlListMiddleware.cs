using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceChassis.Middlewares
{
    //When using this middleware, implement the IAccessControlListRoutes interface
    /// <summary>
    /// Middleware to control access based on X-Source-Ip header enrichment
    /// </summary>
    public class AccessControlListMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessControlListMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var accessControlListRoutes = serviceProvider.GetService<IAccessControlListRoutes>();

            var httpRequest = httpContext.Request;
            var path = httpContext.Request.Path;

            if (accessControlListRoutes.IsPublicRouteWithoutRestrictions(path.Value, httpRequest.Method))
            {
                await _next(httpContext);
                return;
            }

            if (httpRequest.Headers.TryGetValue("X-Source-Ip", out var ipAddress))
            {
                if (accessControlListRoutes.IsIpAddressWhiteListed(ipAddress, path.Value, httpRequest.Method))
                {
                    await _next(httpContext);
                    return;
                }
            }

            throw new UnauthorizedAccessException();
        }
    }
}
