using Easy.Jwt.Core;
using Microsoft.AspNetCore.Http;

namespace Easy.Jwt.Core.Validation
{
    internal interface IRequestValidation
    {
        /// <summary>
        /// Processes the validation.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        Task<PasswordValidationContext> ValidateAsync(HttpContext context);
    }
}
