/* ---------------------------------------------------------------------    
 * 版权所有 (c) 2023 mailhelin@qq.com  保留所有权利。
 *
 * Comment 	    Vision	    Author              Date  
 * ---------    --------    --------            -----------
 * Created		1.0		    mailhelin@qq.com    2023/8/9 16:57:10
 *
 * ------------------------------------------------------------------------------*/

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Easy.Jwt.Core
{
    internal static class ObjectSerializer
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static string Serialize<T>(T o)
        {
            return JsonSerializer.Serialize(o, Default);
        }

        public static T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value, Default);
        }

        public static object Deserialize(string value, Type type)
        {
            return JsonSerializer.Deserialize(value, type, Default);
        }

        public static T DeepClone<T>(T i) 
        {
            var str = Serialize(i);

            return Deserialize<T>(str);
        }

        public static T CopyTo<F, T>(F i)
        {
            var str = Serialize(i);

            return Deserialize<T>(str);
        }
    }
}