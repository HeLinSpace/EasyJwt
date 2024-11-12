using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Easy.Jwt.Core.Validation
{
    internal interface IRequestValidator
    {
        /// <summary>
        /// Processes the validation.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        Task<PasswordValidationContext> ValidateAsync(HttpContext context);
    }
}
