using System.Collections.Specialized;
using System.Security.Claims;

namespace Easy.Jwt.Core;

/// <summary>
/// Class describing the resource owner password validation context
/// </summary>
public class PasswordValidationContext
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// form data
    /// </summary>
    public NameValueCollection FormCollection { get; set; }

    /// <summary>
    /// request headers
    /// </summary>
    public NameValueCollection Headers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<Claim> CustomClaims { get; set; }

    /// <summary>
    /// Custom fields for the token response
    /// </summary>
    public Dictionary<string, object> CustomResponse { get; set; } = new Dictionary<string, object>();

}