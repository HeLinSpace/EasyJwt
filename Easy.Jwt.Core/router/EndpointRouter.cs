/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Easy.Jwt.Core
{
    internal class EndpointRouter : IEndpointRouter
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IEndpointHandler> _handlers;

        public EndpointRouter(IEnumerable<IEndpointHandler> handlers, ILogger<EndpointRouter> logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        public IEndpointHandler Find(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var handler = _handlers.FirstOrDefault(s => s.Equals(context.Request.Path, context.Request.Method));

            if (handler != null)
            {
                var endpointName = handler.EndpointKey;
                _logger.LogDebug("Request path {path} matched to endpoint type {endpoint}", context.Request.Path, endpointName);
            }

            _logger.LogTrace("No endpoint entry found for request path: {path}", context.Request.Path);

            return handler;
        }
    }
}
