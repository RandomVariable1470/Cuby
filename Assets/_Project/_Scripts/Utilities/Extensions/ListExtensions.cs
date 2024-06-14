using System.Collections.Generic;

namespace RV.Util
{
    public static class ListExtensions 
    {
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }
    }
}