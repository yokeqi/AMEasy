using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yokeqi.AMEasy.Winform
{
    public static class Extensions
    {
        public static bool IsEmpty(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool ContainsIgnoreCase(this string src, string tar)
        {
            return src.ToLower().Contains(tar.ToLower());
        }
    }
}
