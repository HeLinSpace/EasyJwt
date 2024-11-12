using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using h.general.extensions;

namespace Easy.Jwt.Core.Validation
{
    internal class RequestValidation : IRequestValidator
    {
        protected readonly JwtSettings _jwtSettings;

        public RequestValidation(ILogger<RequestValidation> logger, JwtSettings jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
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
                validationContext.ClientId = form.Get(JwtConsts.RequestKey.ClientId);
                validationContext.ClientSecret = form.Get(JwtConsts.RequestKey.ClientSecret);
                validationContext.Username = form.Get(JwtConsts.RequestKey.UserName);
                validationContext.Password = form.Get(JwtConsts.RequestKey.Password);

                validationContext.FormCollection = form;
            }
            else if (context.Request.HasJsonContentType())
            {
                var json = await context.Request.ReadFromJsonAsync<JsonElement>();
                var form = new NameValueCollection();
                foreach (JsonProperty property in json.EnumerateObject())
                {
                    var value = property.Value.ValueKind switch
                    {
                        JsonValueKind.String => property.Value.GetString(),
                        JsonValueKind.Number => property.Value.ToString(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Null => null,
                        _ => property.Value.ToString()
                    };

                    form.Add(property.Name, value);
                }

                // 设置用户名和密码
                validationContext = new PasswordValidationContext
                {
                    ClientId = form.Get(JwtConsts.RequestKey.ClientId),
                    ClientSecret = form.Get(JwtConsts.RequestKey.ClientSecret),
                    Username = form.Get(JwtConsts.RequestKey.UserName),
                    Password = form.Get(JwtConsts.RequestKey.Password),
                    FormCollection = form
                };
            }
            else 
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidRequestData);
            }

            if (validationContext.Username.IsMissing())
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.UsernameEmptyError);
            }

            if (validationContext.ClientId.IsMissing() || validationContext.ClientSecret.IsMissing()) 
            {
                throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidClientIdOrClientSecret);
            }

            var client = _jwtSettings.Clients.FirstOrDefault(s => s.ClientId == validationContext.ClientId && s.ClientSecret == validationContext.ClientSecret) ?? throw new HttpRequestException(JwtConsts.JwtGenerateError.InvalidClientIdOrClientSecret);

            validationContext.Client = client;

            validationContext.Headers = context.Request.Headers.AsNameValueCollection();

            return validationContext;
        }
    }
}
