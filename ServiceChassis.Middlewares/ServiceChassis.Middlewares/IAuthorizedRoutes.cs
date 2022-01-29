using System.Security.Claims;

namespace ServiceChassis.Middlewares
{
    /// <summary>
    /// Required in combincation with AuthorizationMiddleware
    /// </summary>
    public interface IAuthorizedRoutes
    {
        /// <summary>
        /// Checks if a route requires authorization or not
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool RequiresAuthorization(string method, string path);

        /// <summary>
        /// Performs an authorization checks based on the principals token
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        bool Authorize(string method, string path, ClaimsPrincipal claimsPrincipal);
    }
}
