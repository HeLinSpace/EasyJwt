namespace Easy.Jwt.Core
{
    public class JwtClient
    {
        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// allowed access scopes
        /// </summary>
        public string[] Scopes { get; set; }

        /// <summary>
        /// audience
        /// default *
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// expires on（seconds），default 8 hours
        /// </summary>
        public int? Expires { get; set; }
    }
}
