using System.Security.Claims;

namespace ServiceChassis.Middlewares.Jwts
{
    /// <summary>
    /// Token handler interface to be seen by services
    /// </summary>
    public interface IJwtTokenHandler
    {
        /// <summary>
        /// Generate new tokens based on old refresh token
        /// </summary>
        /// <param name="oldRefreshToken"></param>
        /// <param name="email"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        (string, string) RefreshTokens(string oldRefreshToken, string email, string serviceName);

        /// <summary>
        /// Generate new tokens for identifier and service name
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        (string, string) GenerateTokens(string identifier, string serviceName);

        /// <summary>
        /// Generate a new access token for the identifier and issuing service name
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        string GenerateToken(string identifier, string serviceName);

        /// <summary>
        /// Verify an input token for particular token type
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        ClaimsPrincipal Verify(string input, TokenType tokenType);
    }
}
