using Easy.Jwt.Core.Validation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class TokenEndpoint : IEndpointHandler
    {
        protected readonly IRequestValidator _requestValidator;
        protected readonly IPasswordValidator _passwordValidator;
        protected readonly ITokenGenerator _generator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestValidator"></param>
        public TokenEndpoint(IRequestValidator requestValidator, IPasswordValidator passwordValidator, ITokenGenerator generator) 
        {
            _requestValidator = requestValidator;
            _passwordValidator = passwordValidator;
            _generator = generator;
        }

        string IEndpointHandler.Method => "Post";

        string IEndpointHandler.Path => "/connect/token";

        string IEndpointHandler.EndpointKey => "connect-token";

        async Task IEndpointHandler.ProcessAsync(HttpContext context)
        {
            var validationContext = await _requestValidator.ValidateAsync(context);

            var validateResult = await _passwordValidator.ValidateAsync(validationContext);

            var tokenResult = new JwtResponse { IsSuccess = false };

            if (validateResult)
            {
                var tokenInfo = _generator.GenerateToken(validationContext.CustomClaims, validationContext.Client);
                tokenResult = new JwtResponse(tokenInfo)
                {
                    Custom = validationContext.CustomResponse
                };
            }
            else
            {
                tokenResult.Error = JwtConsts.JwtGenerateError.UsernameEmptyError;
                tokenResult.Custom = validationContext.CustomResponse;
            }

            await tokenResult.ExecuteAsync(context);
        }
    }
}
