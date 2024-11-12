using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Easy.Jwt.Core.Extensions
{
    internal static class CollectionsExtensions
    {

        [DebuggerStepThrough]
        public static bool IsPresent<T>(this IEnumerable<T> list)
        {
            return (list != null && list.Any());
        }

        [DebuggerStepThrough]
        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return (list == null || !list.Any());
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this object obj)
        {
            return obj != null;
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this object obj)
        {
            return obj == null || string.IsNullOrEmpty(Convert.ToString(obj));
        }
    }
}
