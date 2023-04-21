namespace Easy.Jwt.Core;
public interface IPasswordValidator
{
    Task<bool> ValidateAsync(PasswordValidationContext context);
}
