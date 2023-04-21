using IThink.Bi.Core.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;

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
        public static IServiceCollection AddEasyJwt<T>(this IServiceCollection serviceCollection, Action<JwtSettings>? configure = null) where T : class, IPasswordValidator
        {
            var settings = new JwtSettings();
            configure?.Invoke(settings);
            serviceCollection.AddSingleton(settings);
            serviceCollection.AddTransient<IRequestValidation, RequestValidation>();
            serviceCollection.AddTransient<IPasswordValidator, T>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds IdentityServer to the pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEasyJwt(this IApplicationBuilder app)
        {
            app.Map("/connect/token", builder =>
            {
                Validate(app);
                builder.UseMiddleware<JwtMiddleware>();
            });

            return app;
        }

        private static void Validate(IApplicationBuilder app)
        {
            var requestValidations = app.ApplicationServices.GetService<IRequestValidation>();
            var passwordValidators = app.ApplicationServices.GetService<IPasswordValidator>();
            var jwtSettings = app.ApplicationServices.GetService<JwtSettings>();

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