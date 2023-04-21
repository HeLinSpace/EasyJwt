using IThink.Bi.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Easy.Jwt.Core;

internal class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="logger">The logger.</param>
    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="router">The router.</param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context, IRequestValidation requestValidation, IPasswordValidator passwordValidator, JwtSettings jwtSettings)
    {
        try
        {
            var validationContext = await requestValidation.ValidateAsync(context);

            var validateResult = await passwordValidator.ValidateAsync(validationContext);

            var tokenResult = GenerateToken(context, jwtSettings, validationContext, validateResult);

            await tokenResult.ExecuteAsync(context);
        }
        catch (HttpRequestException ex)
        {
            var res = new JwtResponse
            {
                Error = ex.Message,
            };

            context.Response.StatusCode = 400;
            await res.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            var res = new JwtResponse
            {
                Error = ex.Message,
            };

            context.Response.StatusCode = 500;
            await res.ExecuteAsync(context);
        }
    }

    private static JwtResponse GenerateToken(HttpContext context, JwtSettings jwtSettings, PasswordValidationContext validationContext, bool validateResult)
    {
        var result = new JwtResponse();

        if (validateResult)
        {
            jwtSettings ??= new JwtSettings();

            string token = null;
            string issuer = jwtSettings.Issuer ?? context.Request.Host.Value;
            string audience = jwtSettings.Issuer ?? issuer;

            var resultClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, CommonHelper.NewGuid),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.Now.Ticks.ToString(), ClaimValueTypes.Integer64)
            };

            if (jwtSettings.DefaultClaims.IsPresent())
            {
                resultClaims.AddRange(jwtSettings.DefaultClaims);
            }

            if (validationContext.CustomClaims.IsPresent())
            {
                resultClaims.AddRange(validationContext.CustomClaims);
            }

            DateTime expiresAt;

            if (jwtSettings.Credentials != null)
            {
                token = JWTHelper.GetJwtToken(jwtSettings.Credentials, resultClaims, issuer, audience, jwtSettings.Expires, out expiresAt);
                result.AccessToken = token;
                result.ExpiresAt = expiresAt;
                result.TokenType = jwtSettings.TokenType;
                result.Custom = validationContext.CustomResponse;
            }
            else if (jwtSettings.PrivateKey.IsPresent())
            {
                token = JWTHelper.GetJwtToken(jwtSettings.PrivateKey, resultClaims, issuer, audience, jwtSettings.Expires, out expiresAt);
                result.AccessToken = token;
                result.ExpiresAt = expiresAt;
                result.TokenType = jwtSettings.TokenType;
                result.Custom = validationContext.CustomResponse;
            }
            else
            {
                result.Error = JwtConsts.JwtGenerateError.CredentialError;
            }
        }
        else
        {
            result.Error = JwtConsts.JwtGenerateError.InvalidUsernameOrPassword;
            result.Custom = validationContext.CustomResponse;
        }

        return result;
    }
}
