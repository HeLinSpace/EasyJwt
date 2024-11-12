using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Easy.Jwt.Core.Test.Api
{
    public class PasswordValidator : IPasswordValidator
    {
        public async Task<bool> ValidateAsync(PasswordValidationContext context)
        {
            // do some thing 

            context.CustomClaims = new List<Claim>
            {
                new (JwtClaimTypes.Id,"id"),
                new (JwtClaimTypes.Subject,"id"),
                new ("Username",context.Username),

            };

            context.CustomResponse = new Dictionary<string, object> { { "status", 200 } };

            return true;
        }
    }
}
