namespace Easy.Jwt.Core
{
    internal struct JwtConsts
    {
        internal struct JwtGenerateError
        {
            public const string InitError = "init error: Please call the method serviceCollection.AddJwt() first .";
            public const string PasswordValidatorNotImplementedError = "inject a custom implementation of the IPasswordValidator into the ioc container first .";
            public const string CredentialError = "configuration error: Please call the method serviceCollection.AddJwt() to set the SigningCredentials or PrivateKey(RSAHelper.Generate() may help you) .";
            public const string InvalidUsernameOrPassword = "invalid username or password .";
            public const string InvalidRequest = "invalid request, only post is allowed .";
            public const string InvalidRequestData = "invalid request, only fromform or frombody is allowed .";
        }

        internal struct RequestKey
        {
            public const string UserName = "username";
            public const string Password = "password";
        }
    }
}
