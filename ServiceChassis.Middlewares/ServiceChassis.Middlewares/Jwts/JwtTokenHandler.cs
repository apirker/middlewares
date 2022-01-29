using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ServiceChassis.Middlewares.Jwts
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private string issuer;
        private string key;
        private string audience;

        public const string ClaimIdentifier = "identifier";
        public const string ClaimTokenType = "token_type";
        public const string ClaimIssuingService = "issuing_service";

        private const string type_access_token = "access";
        private const string type_refresh_token = "refresh";

        private int accessTokenValidityPeriodInSeconds;
        private int refreshTokenValidityPeriodInSeconds;

        public JwtTokenHandler(JwtOptions jwtOptions)
        {
            issuer = jwtOptions.Issuer;
            key = jwtOptions.Key;
            audience = jwtOptions.Audience;
            accessTokenValidityPeriodInSeconds = jwtOptions.AccessTokenValidityPeriodInSeconds;
            refreshTokenValidityPeriodInSeconds = jwtOptions.RefreshTokenValidityPeriodInSeconds;
        }

        private string GetTokenTypeString(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Access:
                    return type_access_token;
                case TokenType.Refresh:
                    return type_refresh_token;
            }

            throw new NotSupportedException("token type not supported");
        }

        private string Generate(string email, TokenType tokenType, string issuingService)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtTokenHandler.ClaimIdentifier, email));
            claims.Add(new Claim(JwtTokenHandler.ClaimTokenType, GetTokenTypeString(tokenType)));
            claims.Add(new Claim(JwtTokenHandler.ClaimIssuingService, issuingService));

            var expiry = tokenType == TokenType.Access 
                ? DateTime.Now.AddSeconds(accessTokenValidityPeriodInSeconds)
                : DateTime.Now.AddSeconds(refreshTokenValidityPeriodInSeconds);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal Verify(string input, TokenType tokenType)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(input, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes), 
                ValidAudience = audience,
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var tokenTypeClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtTokenHandler.ClaimTokenType);
            if (tokenTypeClaim == null)
                throw new InvalidOperationException();

            if (tokenTypeClaim.Value != GetTokenTypeString(tokenType))
                throw new InvalidOperationException();

            return principal;
        }

        public (string, string) RefreshTokens(string oldRefreshToken, string email, string serviceName)
        {
            Verify(oldRefreshToken, TokenType.Refresh);

            var accessToken = Generate(email, TokenType.Access, serviceName);
            var refreshToken = Generate(email, TokenType.Refresh, serviceName);

            return (accessToken, refreshToken);
        }

        public (string, string) GenerateTokens(string email, string serviceName)
        {
            var accessToken = Generate(email, TokenType.Access, serviceName);
            var refreshToken = Generate(email, TokenType.Refresh, serviceName);

            return (accessToken, refreshToken);
        }

        public string GenerateToken(string identifier, string serviceName)
        {
            return Generate(identifier, TokenType.Access, serviceName);
        }
    }
}
