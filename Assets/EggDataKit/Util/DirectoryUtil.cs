#region

//文件创建者：Egg
//创建时间：10-25 11:17

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace EggFramework.Util
{
    public static class DirectoryUtil
    {
        public static void MakeSureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static IEnumerable<string> GetFilePaths()
        {
            var dirs = Directory.GetDirectories("Assets/", "*", SearchOption.AllDirectories);
            var filterList = dirs.Where(file =>
                !file.Contains(".git") &&
                !file.Contains("Plugins") &&
                !file.Contains("Scenes")
            ).Select(file => file.Replace('\\', '/')).ToHashSet();
            return filterList;
        }

        public static string ExtractName(string path)
        {
            var startIndex1 = path.LastIndexOf("/",  StringComparison.Ordinal) + 1;
            var startIndex2 = path.LastIndexOf("\\", StringComparison.Ordinal) + 1;
            var startIndex  = Mathf.Max(startIndex1, startIndex2);
            var endIndex    = path.LastIndexOf(".", StringComparison.Ordinal);
            return endIndex == -1 ? path[startIndex..] : path[startIndex..endIndex];
        }

        public static string ExtractFolder(string path)
        {
            var endIndex1 = path.LastIndexOf("/",  StringComparison.Ordinal);
            var endIndex2 = path.LastIndexOf("\\", StringComparison.Ordinal);
            var endIndex  = Mathf.Max(endIndex1, endIndex2);
            return path[..endIndex];
        }
    }
}