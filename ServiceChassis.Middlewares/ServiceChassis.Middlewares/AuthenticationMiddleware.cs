using Microsoft.AspNetCore.Http;
using ServiceChassis.Middlewares.Jwts;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceChassis.Middlewares
{
    //When using this middleware, implement the IAuthenticatedRoutes interface
    /// <summary>
    /// Middleware which performs authentication based on X-Access-Token
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var authenticatedRoutes = serviceProvider.GetService<IAuthenticatedRoutes>();

            var httpRequest = httpContext.Request;
            var path = httpContext.Request.Path;
            var pathArray = path.Value.Split('/');

            if (authenticatedRoutes.IsUnauthenticatedRoute(httpRequest.Method, path.Value))
            {
                await _next(httpContext);
                return;
            }

            var jwtTokenHandler = serviceProvider.GetService<IJwtTokenHandler>();
            if (httpRequest.Headers.TryGetValue("X-Access-Token", out var accessToken))
            {
                jwtTokenHandler.Verify(accessToken, TokenType.Access);
                await _next(httpContext);
                return;
            }

            throw new UnauthorizedAccessException();
        }
    }
}
