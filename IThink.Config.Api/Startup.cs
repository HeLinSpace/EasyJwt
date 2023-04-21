using Easy.Jwt.Core;
using IThink.Config.Api.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IThink.Config.Api
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
            services.AddControllers();

            services.AddSwaggerGen(c => {
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

            services.AddEasyJwt<PasswordValidator>(s =>
            {
                s.PrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCsg8JSzXS0bGvU4UP+Jh1h8rXnE3BP0CIEn3kvE6nq+XpwClRTE8I+sLHc5tCX2cVJqgDV+5bMLnsu6P9CNpvINInHI9qapBVDgGesSIcKCmmGKxo/mI5ElPKZ+rMMmEbYTIGTtIuf8EA6diyxs2w06AY7cnzlofqdj75i4Pd4qys2F/9EbZrivlofsV0CGqwKU6WUKa5wOa1oN416Z+8rdsFSlkTftj283njfRpRsbfZKeGTdrR4VMtKSnF20d3jrisb68Hnzzl//wd97ac7arSRLy75P05nw0/9Io9uKc+xryH/oDYCsa1O+dGMReqyI4xxPpb9gwMcav/z9XUVFAgMBAAECggEAOJmqIfyis6d24YtfAX2D2mUFZWEtUiJZZubjnZx3/U4I68WX0QIEkwjp8i4QdiHO5tJOH+bBnRAK/mMXjKPaDJ+gLKQIzv/Ssijo2s+Y7qRn5ssxe3gUBVBRIE/ues1jQQgo40szHDB5AjVPiKXzdJBDqzFu5PB2B0foEQz6ZK5JvzqkKemq4VEjzgeYBbwgX5kfDZ++UoL987Aij5tNe1h5Wgc3izKz2WMsTIfwaGe4HRjxSBsF0jj5hKyJNlbvD+CEDK/SRKlnNoCrqtgusYTavNg1N6nt+k0isnkkVomI5KhUxO5+ZEQhDpbRemmAxoosMwOmC7xsx/n59vajaQKBgQDAxGcEZUpiuy9GD5zHeUhmmdVOBcTwqF0v2Qnt3cHo7fnCvjGraVSCpE+5ACxci3A9Ehz5q3cgSFJ5JEihYWJVpxqL5e0jcZo0pbBqOhZZxRX+hShw2KYTLge35lp8hvGf5BRQKOAXMrLM7TQozasBtuYu75k5+bRiHLYaJF4crwKBgQDlGqehEisLQfdD4vz2VlCdc22qgegcgDI1C2ET40CAgXinBJZMtlZQA3LAKSdySPOfBHuJSB00SmDpeOw0HGi5XewHZXNcXEymOLWXHwn0vhyJsY2iGA9ikHhzG2CN2u9vVy9Mzk5+nHrQjcUYpuFSSaeNqWQQclRfCN0rGuaCSwKBgEHkYNptzUmGdphaSdsdqBP6TaGH81PYGVJ7vBslDF2UyyU4Yj4XmR23ENFKL9/Kgik+Ac2OQQA1+IsUTgdsBHMG1dowVCkjcfTBFlaZWH6DEguYdMRuKgawW08PXKrobbub+R2Ve1Qyrk/CXWv9MZE6deBhYl0g7/oBmnXBLLJdAoGAfpC8HFp6Fw0JOaKzVq8Wd/Ull3WOsfgMRIuVxLPdlWBxM+tv0M9GXYuaIBhcJ2Gv3KBQUuXY5GNWqIRaEOvE2urNEsTz9wkyev2HLAZErMU95L+G3A5oqW3gbM6qB5P0hFDo24h5iq6NlptLUDQY9Cmme7chhYhzndC6xi5lO4MCgYAwWixszroMOSAzfw3MZQYsmXzDADM2ORWNTmjykVmELXjX4By4Vxtbb6Cy5g2qPaYOCwMbXkbgQDZDwYyFXoKrNmIkeCX1mVYfQXO/fEe1L/teoBw9GSdg2YG/+z5NATn1wYq2M/wiEhhj25JVoCa38bcTkoc78tDu1D+eav7ASA==";

            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JWTHelper.GetRsaSecurityKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArIPCUs10tGxr1OFD/iYdYfK15xNwT9AiBJ95LxOp6vl6cApUUxPCPrCx3ObQl9nFSaoA1fuWzC57Luj/QjabyDSJxyPamqQVQ4BnrEiHCgpphisaP5iORJTymfqzDJhG2EyBk7SLn/BAOnYssbNsNOgGO3J85aH6nY++YuD3eKsrNhf/RG2a4r5aH7FdAhqsClOllCmucDmtaDeNemfvK3bBUpZE37Y9vN5430aUbG32Snhk3a0eFTLSkpxdtHd464rG+vB5885f/8Hfe2nO2q0kS8u+T9OZ8NP/SKPbinPsa8h/6A2ArGtTvnRjEXqsiOMcT6W/YMDHGr/8/V1FRQIDAQAB"),
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseEasyJwt();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
