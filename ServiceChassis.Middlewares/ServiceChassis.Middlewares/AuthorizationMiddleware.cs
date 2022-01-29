using Microsoft.AspNetCore.Http;
using ServiceChassis.Middlewares.Jwts;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceChassis.Middlewares
{
    //When using this middleware, implement the IAuthorizedRoutes interface
    /// <summary>
    /// Middleware which performs authorization based on X-Access-Token
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var httpRequest = httpContext.Request;
            var path = httpContext.Request.Path;

            var authorizedRoutes = serviceProvider.GetService<IAuthorizedRoutes>();
            if (!authorizedRoutes.RequiresAuthorization(httpRequest.Method, path.Value) 
                || path.Value.StartsWith("/swagger"))
            {
                await _next(httpContext);
                return;
            }

            var jwtTokenHandler = serviceProvider.GetService<IJwtTokenHandler>();
            if (httpRequest.Headers.TryGetValue("X-Access-Token", out var accessToken))
            {
                var claimsPrincipal = jwtTokenHandler.Verify(accessToken, TokenType.Access);
                if (authorizedRoutes.Authorize(httpRequest.Method, path.Value, claimsPrincipal))
                {
                    await _next(httpContext);
                    return;
                }
            }

            throw new UnauthorizedAccessException();
        }

    }
}
