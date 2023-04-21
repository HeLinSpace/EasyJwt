using Easy.Jwt.Core;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IThink.Config.Api.Login
{
    public class PasswordValidator : IPasswordValidator
    {
        public async Task<bool> ValidateAsync(PasswordValidationContext context)
        {
            // do some thing 

            context.CustomClaims = new List<System.Security.Claims.Claim>
            {
                new Claim(JwtClaimTypes.Id,"id")
            };
            context.CustomResponse = new Dictionary<string, object> { { "status", 200 } };

            return true;
        }
    }
}
