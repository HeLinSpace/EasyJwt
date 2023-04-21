using Easy.Jwt.Core;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IThink.Bi.Core.Validation
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
