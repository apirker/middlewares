namespace ServiceChassis.Middlewares
{
    /// <summary>
    /// Required in combincation with AuthenticationMiddleware
    /// </summary>
    public interface IAuthenticatedRoutes
    {
        /// <summary>
        /// Checks if the provided route is reachable for everyone
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsUnauthenticatedRoute(string method, string path);

    }
}
