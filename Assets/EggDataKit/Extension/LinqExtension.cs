#region

//文件创建者：Egg
//创建时间：09-17 01:27

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace EggFramework
{
    public static class LinqExtension
    {
        public static bool AddIfNotExist<T>(this List<T> list, T element)
        {
            if (list.Contains(element)) return false;
            list.Add(element);
            return true;
        }

        public static bool AddIfNotExist<T>(this List<T> list, T element, Func<T, T, bool> equalFunc)
        {
            if (list.Any((ele) => equalFunc != null && equalFunc.Invoke(ele, element))) return false;
            list.Add(element);
            return true;
        }
    }
}