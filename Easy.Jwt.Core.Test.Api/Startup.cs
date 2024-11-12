using Easy.Jwt.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Easy.Jwt.Core.Test.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            services.AddSwaggerGen(c =>
            {
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

            var discHost = "http://localhost:22002";

            services.AddEasyJwtServer<PasswordValidator>(s =>
            {
                s.SetDeveloperSigningCredential();
                s.Issuer = discHost;
                s.Expires = 5 * 60;
                // ... other settings
                s.Clients = new List<JwtClient>
                {
                    new(){ ClientId = "client_id", ClientSecret = "client_secret",Audience = "*" , Scopes = new string[]{ "api1"} }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1"); 
                });
            });

            services.AddHttpClient("NoCertificate").ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(options =>
            {
                options.Authority = discHost;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = discHost,// can edit in JwtSettings
                    ValidAudience = "*"// can edit in JwtSettings
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        var _httpClientFactory = ctx.HttpContext.RequestServices.GetService<IHttpClientFactory>();

                        // 计算总时长和已过时长
                        var nbf = Convert.ToInt64(ctx.Principal.Claims.GetClaimValue("nbf"));
                        var exp = Convert.ToInt64(ctx.Principal.Claims.GetClaimValue("exp"));
                        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                        // 计算是否超过有效期的 70%
                        if ((now - nbf) > (exp - nbf) * 0.7)
                        {
                            // 需要刷新 token
                            var client = _httpClientFactory.CreateClient("NoCertificate");
                            var disco = client.GetDiscoveryDocumentAsync(discHost).Result;

                            var token = client.GenerateTokenAsync(disco.TokenEndpoint, new TokenRequest
                            {
                                ClientId = "client_id",
                                ClientSecret = "client_secret",
                                Username = ctx.Principal.Claims.GetClaimValue("Username"),
                                CustomProperty = new Dictionary<string, object>
                                {
                                    { "language", "zh" },
                                    { "refresh", true },
                                }
                            }).Result;

                            if (!token.IsSuccess)
                            {
                                ctx.Fail("令牌验证失败");
                                return Task.CompletedTask;
                            }

                            ctx.HttpContext.Response.Headers.Add("X-Authorization", token.AccessToken);
                        }

                        //ctx.HttpContext.SetWorkRequest();
                        return Task.CompletedTask;
                    },
                };
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Easy.Jwt.Core.Test.Api v1"));
            }

            app.UseRouting();

            app.UseEasyJwt();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
