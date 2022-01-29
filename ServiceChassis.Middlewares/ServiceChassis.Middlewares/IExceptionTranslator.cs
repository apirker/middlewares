using System;
using System.Net;

namespace ServiceChassis.Middlewares
{
    /// <summary>
    /// Required in combination with ExceptionMiddleware
    /// </summary>
    public interface IExceptionTranslator
    {
        /// <summary>
        /// Translates an exception into a HTTP error code
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        HttpStatusCode HttpCodeForException(Exception e);
    }
}
