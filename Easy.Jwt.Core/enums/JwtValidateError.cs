
namespace Easy.Jwt.Core;

/// <summary>
/// enum for jwt validate error
/// </summary>
public enum JwtValidateError
{
    /// <summary>
    /// 未知错误
    /// </summary>
    UnKnownError = 0,

    /// <summary>
    /// 无效token
    /// </summary>
    TokenError = 1,

    /// <summary>
    /// token过期
    /// </summary>
    ExpiredError = 2,

    /// <summary>
    /// replace a record
    /// </summary>
    AudienceError = 3,
}