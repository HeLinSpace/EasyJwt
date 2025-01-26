using Easy.Jwt.Core.Extensions;
using Easy.Jwt.Core.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Easy.Jwt.Core
{
    public static class Core
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyJwtServer<T>(this IServiceCollection serviceCollection, Action<JwtSettings> configure = null) where T : class, IPasswordValidator
        {
            var settings = new JwtSettings
            {
                Issuer = "*",
                Expires = 28800,
                TokenType = "Bearer"
            };

            configure?.Invoke(settings);

            if (settings.Authority.IsPresent())
            {
                BusinessException.Throw(!CommonHelper.IsValidScheme(settings.Authority), "'Authority' does not contain 'Scheme' information .");
            }

            BusinessException.Throw(settings.Clients.IsEmpty() && settings.ClientFunc.IsEmpty(), "no client hear .");

            serviceCollection.AddSingleton(settings);
            serviceCollection.AddScoped<IRequestValidator, RequestValidation>();
            serviceCollection.AddScoped<IPasswordValidator, T>();
            serviceCollection.AddScoped<ITokenGenerator, JWTGenerator>();

            serviceCollection.AddScoped<IEndpointRouter, EndpointRouter>();
            serviceCollection.AddScoped<IEndpointHandler, TokenEndpoint>();
            serviceCollection.AddScoped<IEndpointHandler, DiscoveryEndpoint>();
            serviceCollection.AddScoped<IEndpointHandler, DiscoveryKeyEndpoint>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds IdentityServer to the pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEasyJwt(this IApplicationBuilder app)
        {
            Validate(app);
            app.UseMiddleware<JwtMiddleware>();

            return app;
        }

        private static void Validate(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var requestValidations = scope.ServiceProvider.GetService<IRequestValidator>();
            var passwordValidators = scope.ServiceProvider.GetService<IPasswordValidator>();
            var jwtSettings = scope.ServiceProvider.GetService<JwtSettings>();

            if (jwtSettings.ClientFunc.IsPresent())
            {
                jwtSettings.Clients = jwtSettings.ClientFunc.Invoke(app.ApplicationServices);
                BusinessException.Throw(jwtSettings.Clients.IsEmpty(), "no client hear .");
            }

            if (requestValidations == null || jwtSettings == null)
            {
                throw new InvalidOperationException(JwtConsts.JwtGenerateError.InitError);
            }

            if (passwordValidators == null)
            {
                throw new InvalidOperationException(JwtConsts.JwtGenerateError.PasswordValidatorNotImplementedError);
            }
        }
    }
}