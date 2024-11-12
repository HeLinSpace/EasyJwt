/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Http;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// The endpoint router
    /// </summary>
    public interface IEndpointRouter
    {
        /// <summary>
        /// Finds a matching endpoint.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        IEndpointHandler Find(HttpContext context);
    }
}