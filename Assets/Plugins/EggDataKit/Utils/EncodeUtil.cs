#region

//文件创建者：Egg
//创建时间：12-29 08:54

#endregion

#if UNITY_EDITOR

using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace EggFramework.Util
{
    public static class EncodeUtil
    {
        public static void RefreshEncode()
        {
            var files = DirectoryUtil.GetFileEndWith(".py");
            foreach (var file in files)
            {
                if (DirectoryUtil.ExtractName(file) == "GBKtoUTF8")
                {
                    // 1. 使用完整路径调用 Python
                    string pythonPath = "C:\\Users\\lenovo\\AppData\\Local\\Programs\\Python\\Python313\\python.exe"; // 或指定绝对路径如 @"C:\Python39\python.exe"
                    string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), file);
                    string targetDir = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Excel/Text");

                    // 2. 直接调用 Python 进程（无需 cmd.exe）
                    var process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = $"\"{scriptPath}\" \"{targetDir}\"", // 用引号包裹路径
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                        Debug.LogError($"Python Error: {error}");

                    //Debug.Log($"Exit Code: {process.ExitCode}");

                    // 4. 强制刷新 Unity 资源
                    AssetDatabase.Refresh();
                    break;
                }
            }
        }
    }
}
#endif