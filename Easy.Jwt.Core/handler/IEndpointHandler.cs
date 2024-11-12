/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// Endpoint handler
    /// </summary>
    public interface IEndpointHandler
    {
        public string Method { get; }

        public string Path { get; }
        public string EndpointKey { get; }

        public bool Equals(string path, string method)
        {
            return Path.Equals(path, StringComparison.OrdinalIgnoreCase) && Method.Equals(method, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public Task ProcessAsync(HttpContext context);
    }
}