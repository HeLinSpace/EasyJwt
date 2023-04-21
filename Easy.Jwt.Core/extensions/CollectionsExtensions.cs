using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Easy.Jwt.Core;

internal static class CollectionsExtensions
{

    [DebuggerStepThrough]
    public static bool IsPresent<T>(this IEnumerable<T> list)
    {
        return (list != null && list.Any());
    }

    [DebuggerStepThrough]
    public static bool IsPresent<T>(this object? obj) where T : class
    {
        return obj != null;
    }
}