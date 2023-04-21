using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Jwt.Core;

internal class CommonHelper
{
    /// <summary>
    /// 生成新的Guid
    /// </summary>
    /// <returns></returns>
    public static string NewGuid
    {
        get
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
