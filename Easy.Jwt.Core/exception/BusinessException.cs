/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using Easy.Jwt.Core.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// 业务异常
    /// </summary>
    internal class BusinessException : Exception
    {
        /// <summary>
        /// 状态Code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public BusinessException()
        {
        }

        public static void ThrowIfNull(object argument, params string[] args)
        {
            if (argument is null || string.IsNullOrEmpty(Convert.ToString(argument)))
            {
                var message = string.Format(JwtConsts.JwtGenerateError.NullOrEmptyError, args);

                throw new BusinessException(message, 400);
            }
        }

        public static void ThrowIfNull(object argument, int code, string message)
        {
            if (argument is null || string.IsNullOrEmpty(Convert.ToString(argument)))
            {
                throw new BusinessException(message, code);
            }
        }

        public static void Throw(Func<bool> func, string message, int code = 500)
        {
            if (func())
            {
                throw new BusinessException(message, code);
            }
        }

        public static void Throw(bool error, string message, int code = 500)
        {
            if (error)
            {
                throw new BusinessException(message, code);
            }
        }

        public static void Throw(string message, int code = 500)
        {
            throw new BusinessException(message, code);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public BusinessException(string message, int code = 400)
            : base(message)
        {
            StatusCode = code;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="innerException"></param>
        /// <param name="statusCode"></param>
        public BusinessException(Exception innerException, string message, int statusCode = 500)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ObjectSerializer.Serialize(this);
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            await context.Response.WriteJsonObjectAsync(this);
        }
    }
}
