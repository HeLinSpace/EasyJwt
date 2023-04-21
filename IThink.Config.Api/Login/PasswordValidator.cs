using Easy.Jwt.Core;

namespace IThink.Config.Api.Login
{
    public class PasswordValidator : IPasswordValidator
    {
        public async Task<bool> ValidateAsync(PasswordValidationContext context)
        {
            context.CustomResponse = new Dictionary<string, object> { { "status", 200 } };
           return true;
        }
    }
}
