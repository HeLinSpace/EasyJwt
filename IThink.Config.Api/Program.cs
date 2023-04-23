using Easy.Jwt.Core;
using IThink.Config.Api.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RSAExtensions;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var aaa = RSAHelper.Generate(RSAKeyType.Pkcs8);
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
        });
        builder.Services.AddEasyJwt<PasswordValidator>(s =>
        {
            s.IssuerSigningKey = "iYdYfK15xNwT9AiBJ95LxOp6vl6cApUUxPCPrCx3ObQl9nFSaoA1fuWzC57Luj";

        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";

        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iYdYfK15xNwT9AiBJ95LxOp6vl6cApUUxPCPrCx3ObQl9nFSaoA1fuWzC57Luj")),
                ValidateIssuer = true,
                ValidIssuer = "localhost:12187",
                ValidAudience = "localhost:12187"
            };

            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = ctx =>
                {
                    return Task.CompletedTask;
                },
                OnTokenValidated = ctx =>
                {
                    //ctx.HttpContext.SetWorkRequest();
                    return Task.CompletedTask;
                },
            };
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseEasyJwt();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

