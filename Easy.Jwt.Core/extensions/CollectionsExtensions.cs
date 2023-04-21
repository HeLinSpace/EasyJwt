using System.Diagnostics;

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