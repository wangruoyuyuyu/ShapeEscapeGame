using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public static class AssetUnpacker
    {
        // 解压回调（保持不变）
        public static event Action<float> OnProgressUpdated;
        public static event Action OnUnpackCompleted;
        public static event Action<string> OnErrorOccurred;

        /// <summary>
        /// 从清单文件获取路径并解压所有文件
        /// </summary>
        public static async void UnpackFromList(string targetPath = null)
        {
            try
            {
                string streamingPath = Application.streamingAssetsPath;
                string listFilePath = Path.Combine(streamingPath, "file_list.json");

                // 1. 先读取清单文件
                List<string> fileRelativePaths = await ReadFileList(listFilePath);
                if (fileRelativePaths == null || fileRelativePaths.Count == 0)
                {
                    Debug.Log("清单文件为空或不存在");
                    OnUnpackCompleted?.Invoke();
                    return;
                }

                // 2. 处理目标路径
                if (string.IsNullOrEmpty(targetPath))
                {
                    targetPath = Application.persistentDataPath;
                }
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                // 3. 逐个解压文件
                for (int i = 0; i < fileRelativePaths.Count; i++)
                {
                    string relativePath = fileRelativePaths[i];
                    string assetPath = Path.Combine(streamingPath, relativePath).Replace("\\", "/");
                    string targetFilePath = Path.Combine(targetPath, relativePath).Replace("\\", "/");

                    // 更新进度
                    OnProgressUpdated?.Invoke((float)i / fileRelativePaths.Count);

                    // 创建目标目录
                    string targetDir = Path.GetDirectoryName(targetFilePath);
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }

                    // 解压文件
                    if (assetPath.EndsWith(".meta"))
                    {
                        continue;//meta不复制
                    }
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        await CopyAndroidAsset(assetPath, targetFilePath);
                    }
                    else
                    {
                        File.Copy(assetPath, targetFilePath, true);
                        Debug.Log($"已复制：{targetFilePath}");
                    }
                }

                OnProgressUpdated?.Invoke(1.0f);
                OnUnpackCompleted?.Invoke();
            }
            catch (Exception e)
            {
                OnErrorOccurred?.Invoke($"解压失败: {e.Message}");
                Debug.LogError($"解压异常: {e.Message}");
            }
        }

        /// <summary>
        /// 读取清单文件（file_list.json）
        /// </summary>
        private static async Task<List<string>> ReadFileList(string listFilePath)
        {
            try
            {
                // Android平台用UnityWebRequest读取清单
                if (Application.platform == RuntimePlatform.Android)
                {
                    using (UnityWebRequest www = UnityWebRequest.Get(listFilePath))
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        www.SendWebRequest().completed += _ => tcs.SetResult(true);
                        await tcs.Task;

                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError($"读取清单失败: {www.error}");
                            return null;
                        }

                        // 解析JSON
                        FileList fileList = JsonUtility.FromJson<FileList>(www.downloadHandler.text);
                        return fileList.paths;
                    }
                }
                // 其他平台直接读取文件
                else
                {
                    if (!File.Exists(listFilePath))
                    {
                        Debug.LogError($"清单文件不存在: {listFilePath}");
                        return null;
                    }

                    string json = File.ReadAllText(listFilePath);
                    FileList fileList = JsonUtility.FromJson<FileList>(json);
                    return fileList.paths;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"解析清单异常: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Android平台复制文件（保持不变）
        /// </summary>
        private static Task CopyAndroidAsset(string assetPath, string targetPath)
        {
            var tcs = new TaskCompletionSource<bool>();
            UnityWebRequest www = UnityWebRequest.Get(assetPath);

            www.SendWebRequest().completed += _ =>
            {
                try
                {
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes(targetPath, www.downloadHandler.data);
                        Debug.Log($"Android写入成功: {targetPath}（大小: {www.downloadHandler.data.Length}）");
                        tcs.SetResult(true);
                    }
                    else
                    {
                        Debug.LogError($"Android读取失败: {assetPath}，错误: {www.error}");
                        tcs.SetException(new Exception(www.error));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Android处理失败: {e.Message}");
                    tcs.SetException(e);
                }
                finally
                {
                    www.Dispose();
                }
            };

            return tcs.Task;
        }

        // 用于解析清单的辅助类
        [Serializable]
        private class FileList
        {
            public List<string> paths;
        }
    }
}
