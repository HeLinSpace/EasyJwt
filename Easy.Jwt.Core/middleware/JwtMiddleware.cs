/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Reflection.Emit;
using System;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IEndpointRouter endpointRouter)
        {
            try
            {
                var handler = endpointRouter.Find(context);

                if (handler != null)
                {
                    _logger.LogInformation($"invoking endpoint: {handler.GetType().FullName} for {context.Request.Path}");

                    await handler.ProcessAsync(context);

                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                var res = new JwtResponse
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };

                context.Response.StatusCode = 400;
                await res.ExecuteAsync(context);
                return;
            }
            catch (Exception ex)
            {
                var res = new JwtResponse
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };

                context.Response.StatusCode = 500;
                await res.ExecuteAsync(context);
                return;
            }
            await _next(context);
        }
    }
}

