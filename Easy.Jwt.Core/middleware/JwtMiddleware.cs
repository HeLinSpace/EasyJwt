using Easy.Jwt.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
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
        public async Task Invoke(HttpContext context, IRequestValidation requestValidation, IPasswordValidator passwordValidator, JwtSettings jwtSettings, ITokenGenerator generator)
        {
            try
            {
                var validationContext = await requestValidation.ValidateAsync(context);

                var validateResult = await passwordValidator.ValidateAsync(validationContext);

                var tokenResult = GenerateToken(jwtSettings, generator, validationContext, validateResult);

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

        private static JwtResponse GenerateToken(JwtSettings jwtSettings, ITokenGenerator generator, PasswordValidationContext validationContext, bool validateResult)
        {
            var result = new JwtResponse();

            if (validateResult)
            {
                var claims = new List<Claim>
                {
                    new(JwtClaimTypes.Subject, CommonHelper.NewGuid),
                    new(JwtClaimTypes.AuthenticationTime, DateTime.Now.Ticks.ToString(), ClaimValueTypes.Integer64)
                };

                if (jwtSettings.DefaultClaims.IsPresent())
                {
                    claims.AddRange(jwtSettings.DefaultClaims);
                }

                if (validationContext.CustomClaims.IsPresent())
                {
                    claims.AddRange(validationContext.CustomClaims);
                }

                var tokenInfo = generator.GenerateToken(claims);
                result = new JwtResponse(tokenInfo)
                {
                    Custom = validationContext.CustomResponse
                };
            }
            else
            {
                result.Error = JwtConsts.JwtGenerateError.InvalidUsernameOrPassword;
                result.Custom = validationContext.CustomResponse;
            }

            return result;
        }
    }
}
