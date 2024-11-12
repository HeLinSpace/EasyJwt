/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using System.Net.Http;

namespace Easy.Jwt.Core
{
    public interface IMiddlewareEndpoint
    {
        string Method { get; }

        string Path { get; }
        string EndpointKey { get; }

        bool Equals(string path, string method)
        {
            return Path.Equals(path) && Method.Equals(method);
        }
    }
}