namespace ServiceChassis.Middlewares
{
    /// <summary>
    /// Required in combincation with AccessControlListMiddleware
    /// </summary>
    public interface IAccessControlListRoutes
    {
        /// <summary>
        /// Checks if a route is reachable from all ip addresses
        /// </summary>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool IsPublicRouteWithoutRestrictions(string path, string method);

        /// <summary>
        /// Checks if a protected route is reachable from the provided IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool IsIpAddressWhiteListed(string ipAddress, string path, string method);
    }
}
