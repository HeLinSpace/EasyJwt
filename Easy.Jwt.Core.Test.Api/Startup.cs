using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
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
            var publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmaBsfNZX7elksnPT23OYjcMQh8+gqY2LrhBeUiGDx2IlmCkmpi22TEfMVqNR5cj1BawFL0/eK5pilk97vHNFYEM15FeBPVpi6bGOctz9Wxb15zoIm/ab49vMXl6zK6T22s+zO0PWBcW7axO/LhlVJ3ZJEloJVpk5b8EtMcMhM0/zylhJgPLvWoUrd1Bo3YMSFdml6cfP18zhByWI/j2cxqqa28q7/CxF3WgqOvsPENtQM+djPbOmpIsRK2ekPfI+/Y9YZofn/ZweQrsLX95FJ5/fuHQhsbblHgZY0uu1Te2coM3pumeGV6ku78sJV8/cmc/yzOR6mrblfS+IUJtOqQIDAQAB";

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
                //s.IssuerSigningKey = "iYdYfK15xNwT9AiBJ95LxOp6vl6cApUUxPCPrCx3ObQl9nFSaoA1fuWzC57Luj";
                s.PrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCZoGx81lft6WSyc9Pbc5iNwxCHz6CpjYuuEF5SIYPHYiWYKSamLbZMR8xWo1HlyPUFrAUvT94rmmKWT3u8c0VgQzXkV4E9WmLpsY5y3P1bFvXnOgib9pvj28xeXrMrpPbaz7M7Q9YFxbtrE78uGVUndkkSWglWmTlvwS0xwyEzT/PKWEmA8u9ahSt3UGjdgxIV2aXpx8/XzOEHJYj+PZzGqprbyrv8LEXdaCo6+w8Q21Az52M9s6akixErZ6Q98j79j1hmh+f9nB5Cuwtf3kUnn9+4dCGxtuUeBljS67VN7Zygzem6Z4ZXqS7vywlXz9yZz/LM5HqatuV9L4hQm06pAgMBAAECggEAB4K0SxYfjl4Osjq/bMcl+ihDMqCP9joZThU2x0UkMCc4N0wru4wYkXohH0s0wcslT3WtezaoYfFWSIryPbBU//PJOrSceOdmVo57rFnpeP6SXw+TRCy2SlmAkEyI7eVOvDvTQ155pCaalw2MFzDd3OQHhoIoWrd6+1+yWfB0OxHmML/2rGoRYfRKmKCbkPC2Xs2nmf7oK8Ki6i0qJdrMXeH4hJQEbnta7PJOXXbl2D4hFgRMsvE4qe+v++SZZj5LMvx56orloCwtNnYWF6d7tWCnclnqK+RQ1gXMVeuszhWtT5umrZegFqxcKxAksC/rRgbrFlIeAkP5GndGMoKVUQKBgQDDiQtS8X/byjWlzbvwbe+2wrWErBjJNioEFdiha5jUbpx9TAsBRPk2AK92PeEI2rWP8RXJKypy03hbXLC+OMmY6Q9CdXxw4dxMHxbef5NmFFiLaAE5Vswk0k4h3WZDxOEA6pCZSid0B5f9XPUKKbBquFZ4AW1X6sfGQovAdZ5SBwKBgQDJIc7EWlYrNl5/1hFrNWLVIZneyr8nBHzvu+u4Wa3fntHWueS46v3lVMr5rg82ijRYsuNIULRhRGu5rRwbzjZGvKLZnZBn14fHs4R9mJN4u/+sSKtvZtzzPrmfz3zqhfl7AowSp/8O7t9ttF58b+l5txy03PJ5lob7jvvv+4ZtzwKBgBy4Ult6cUc9KigSUdp8q0ryymqURIe5Vu6Gruz2utno7T9SHgOvt9MAwsThTd5ZEXX0+Tg1COUYvoLc2HD4MlQtVzpxuzdxVUQxBNDleCb0MhU3z4Y2g4GdCjDbLhwiHNJfoaGCTM00GFT0hGFbwjfKn5i8zOyu5WzvdUttezWdAoGABdl32/oMm4EfSnR9WTLaRvyNLw+Sc54LzCUY++mPxvsleys43a/n5m2K4awQQTPPxeyd85J1TIbi4ymHsb9TkMcXrWCJvJtmFYbBCa4QS+ibzPToF3tUXbarS1yLc36l/M/cJwa/wj1sPhKK1O+kSKfsq83pN2QLVPGZnicgh70CgYB+8Blx/C+N/LwPSiAQuzPGaWRMinJvfrVNGk/la2yBB1CDNAEGqCHoJ7A97as4HfSePL/pHerJR/b8c+cwptREhA3MPeYXWW6VVSTdCaviJAfadKSmEnIF1uu+d7DHsScVIJF34idNy30OkuF4L3M/rR8IRHj0l4V5tChUg1h+6g==";
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
                    IssuerSigningKey = JWTHelper.GetRsaSecurityKey(publicKey),
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iYdYfK15xNwT9AiBJ95LxOp6vl6cApUUxPCPrCx3ObQl9nFSaoA1fuWzC57Luj")),
                    ValidateIssuer = true,
                    ValidIssuer = "*",
                    ValidAudience = "*"
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        var generator = ctx.HttpContext.RequestServices.GetService<ITokenGenerator>();

                        var token = generator.GenerateToken(ctx.Principal.Claims);

                        ctx.HttpContext.Response.Headers.Add("X-Authorization", token.AccessToken);

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
