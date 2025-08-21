using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateFileList
{
    // 在Unity菜单中添加选项：Assets -> Generate StreamingAssets File List
    [MenuItem("Assets/Generate StreamingAssets File List")]
    public static void Generate()
    {
        // StreamingAssets目录路径
        string streamingPath = Application.streamingAssetsPath;
        
        if (!Directory.Exists(streamingPath))
        {
            Debug.LogError("StreamingAssets目录不存在！");
            return;
        }

        // 递归扫描所有文件，获取相对路径
        List<string> filePaths = new List<string>();
        ScanDirectory(streamingPath, streamingPath, filePaths);

        // 生成清单文件内容（JSON格式）
        string json = JsonUtility.ToJson(new FileList { paths = filePaths }, true);
        
        // 清单文件保存路径（放在StreamingAssets根目录）
        string listFilePath = Path.Combine(streamingPath, "file_list.json");
        File.WriteAllText(listFilePath, json);
        
        Debug.Log($"成功生成文件清单，包含 {filePaths.Count} 个文件：{listFilePath}");
        AssetDatabase.Refresh(); // 刷新Unity资源窗口
    }

    // 递归扫描目录，收集所有文件的相对路径
    private static void ScanDirectory(string rootPath, string currentPath, List<string> filePaths)
    {
        // 获取当前目录下的所有文件
        string[] files = Directory.GetFiles(currentPath);
        foreach (string file in files)
        {
            // 排除清单文件本身（避免递归包含）
            if (Path.GetFileName(file) == "file_list.json")
                continue;
            
            // 计算相对于StreamingAssets的相对路径（如 "data/config.json"）
            string relativePath = Path.GetRelativePath(rootPath, file).Replace("\\", "/");
            filePaths.Add(relativePath);
        }

        // 递归扫描子目录
        string[] directories = Directory.GetDirectories(currentPath);
        foreach (string dir in directories)
        {
            ScanDirectory(rootPath, dir, filePaths);
        }
    }

    // 用于序列化的辅助类（必须标记为[Serializable]）
    [Serializable]
    private class FileList
    {
        public List<string> paths;
    }
}
