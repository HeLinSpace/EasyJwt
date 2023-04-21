using Easy.Jwt.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace IThink.Bi.Core.Validation
{
    internal class RequestValidation : IRequestValidation
    {
        public RequestValidation(ILogger<RequestValidation> logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="BusinessException"></exception>
        public async Task<PasswordValidationContext> ValidateAsync(HttpContext context)
        {
            _logger.LogInformation($"start validate request");

            if (!HttpMethods.IsPost(context.Request.Method))
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidRequest);
            }

            var validationContext = new PasswordValidationContext();

            if (context.Request.HasFormContentType)
            {
                var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
                validationContext.Username = form.Get(JwtConsts.RequestKey.UserName);
                validationContext.Password = form.Get(JwtConsts.RequestKey.Password);

                validationContext.FormCollection = form;
            }
            else if (context.Request.HasJsonContentType())
            {
                validationContext = await context.Request.ReadFromJsonAsync<PasswordValidationContext>();
            }
            else 
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidRequestData);
            }

            if (validationContext.Username.IsMissing() || validationContext.Password.IsMissing()) 
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidUsernameOrPassword);
            }

            validationContext.Headers = context.Request.Headers.AsNameValueCollection();

            return validationContext;
        }
    }
}
