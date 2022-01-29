using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceChassis.Middlewares
{
    public class RouteHelpers
    {
        public static bool IsDeleteMethod(string method)
        {
            return method.ToLower() == "delete";
        }

        public static bool IsGetMethod(string method)
        {
            return method.ToLower() == "get";
        }

        public static bool IsPostMethod(string method)
        {
            return method.ToLower() == "post";
        }

        public static bool IsPutMethod(string method)
        {
            return method.ToLower() == "put";
        }
    }
}
