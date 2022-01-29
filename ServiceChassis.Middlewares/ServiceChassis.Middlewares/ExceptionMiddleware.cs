using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ServiceChassis.Middlewares
{
    //When using this middleware, implement the IExceptionTranslator interface
    /// <summary>
    /// Exception shield middleware performs exception translation into error codes
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception e)
            {

                var translator = httpContext.RequestServices.GetService<IExceptionTranslator>();
                httpContext.Response.StatusCode = (int) translator.HttpCodeForException(e);

                await httpContext.Response.WriteAsync("A server error occured while processing the request.");
            }
        }
    }
}
