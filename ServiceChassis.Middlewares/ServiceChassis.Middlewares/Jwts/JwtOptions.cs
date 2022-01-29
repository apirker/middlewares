namespace ServiceChassis.Middlewares.Jwts
{
    /// <summary>
    /// Options currently avaible for parametrizing the token handler
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Issuer of token
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience of token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Key used for signing the token
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Validity period of access tokens
        /// </summary>
        public int AccessTokenValidityPeriodInSeconds { get; set; }

        /// <summary>
        /// Validity period of refresh tokens
        /// </summary>
        public int RefreshTokenValidityPeriodInSeconds { get; set; }

    }
}
