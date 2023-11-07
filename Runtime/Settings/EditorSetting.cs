// *********************************************************************************
// # Project: JFramework
// # Unity: 2022.3.5f1c1
// # Author: Charlotte
// # Version: 1.0.0
// # History: 2023-10-24  23:56
// # Copyright: 2023, Charlotte
// # Description: This is an automatically generated comment.
// *********************************************************************************

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JFramework.Core;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Debug = UnityEngine.Debug;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JFramework.Editor
{
    public class EditorSetting : OdinMenuEditorWindow
    {
        private static readonly SortedDictionary<int, (string, object)> items = new SortedDictionary<int, (string, object)>();

        /// <summary>
        /// 获取编辑器窗口面板
        /// </summary>
        [MenuItem("Tools/JFramework/Editor Setting _F1", priority = 3)]
        protected static void ShowEditorWindow()
        {
            var window = GetWindow<EditorSetting>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        /// <summary>
        /// 生成窗口
        /// </summary>
        protected override OdinMenuTree BuildMenuTree()
        {
            Add(3, "资源", AssetSetting.Instance);
            Add(2, "设置", GlobalSetting.Instance);
            var tree = new OdinMenuTree(false);
            foreach (var (id, (path, item)) in items)
            {
                switch (id)
                {
                    case 1:
                        tree.Add(path, item, EditorIcons.House);
                        break;
                    case 2:
                        tree.Add(path, item, EditorIcons.SettingsCog);
                        break;
                    case 3:
                        tree.Add(path, item, EditorIcons.Bell);
                        break;
                    default:
                        tree.Add(path, item, EditorIcons.Folder);
                        break;
                }
            }

            return tree;
        }

        /// <summary>
        /// 调用该方法增加面板
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="item"></param>
        public static void Add(int id, string name, object item) => items[id] = (name, item);

        /// <summary>
        /// 更新 AssetBundles 标签
        /// </summary>
        [MenuItem("Tools/JFramework/Update Assets", priority = 1)]
        private static void UpdateAsset()
        {
            var bundleNames = AssetDatabase.GetAllAssetBundleNames();

            foreach (var bundleName in bundleNames)
            {
                if (AssetDatabase.GetAssetPathsFromAssetBundle(bundleName).Length == 0)
                {
                    AssetDatabase.RemoveAssetBundleName(bundleName, true);
                }
            }

            AssetSetting.Instance.sceneAssets.Clear();
            foreach (var folderAsset in AssetSetting.folderAssets)
            {
                var folderPath = AssetDatabase.GetAssetPath(folderAsset);
                var guids = AssetDatabase.FindAssets("t:Object", new[] { folderPath });
                var paths = guids.Select(AssetDatabase.GUIDToAssetPath).Where(path => !AssetDatabase.IsValidFolder(path)).ToList();
                foreach (var path in paths)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer == null) continue;
                    if (importer.assetBundleName != folderAsset.name.ToLower())
                    {
                        Debug.Log($"增加 AssetBundles 资源: {path.Green()}");
                        importer.assetBundleName = folderAsset.name;
                        importer.SaveAndReimport();
                    }

                    var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj is SceneAsset)
                    {
                        AssetSetting.Instance.sceneAssets.Add(path);
                    }

                    if (obj == null) continue;
                    var name = $"{folderAsset.name}/{obj.name}";
                    AssetSetting.Instance.objects[name] = obj;
                }
            }

            Debug.Log($"更新 AssetBundles 完成。".Green());
            AssetSetting.Instance.Save();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 为本地加载的字典赋值
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoad()
        {
            UpdateAsset();
        }

        /// <summary>
        /// 构建 AssetBundles
        /// </summary>
        [MenuItem("Tools/JFramework/Build Assets", priority = 2)]
        private static void BuildAsset()
        {
            UpdateAsset();
            var directory = Directory.CreateDirectory(AssetSetting.platformPath);
            var assetBundleOption = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildPipeline.BuildAssetBundles(AssetSetting.platformPath, assetBundleOption, (BuildTarget)GlobalSetting.Instance.platform);
            var fileInfos = directory.GetFiles().Where(info => info.Extension == "").ToList();
            var fileList = fileInfos.Select(info => new AssetData(GetMD5(info.FullName), info.Name, info.Length)).ToList();
            var saveJson = JsonConvert.SerializeObject(fileList);
            File.WriteAllText(AssetSetting.assetBundleInfo, saveJson);
            Debug.Log("构建 AssetBundles 成功!".Green());
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取MD5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetMD5(string filePath)
        {
            using var file = new FileStream(filePath, FileMode.Open);
            var md5 = new MD5CryptoServiceProvider();
            var md5Info = md5.ComputeHash(file);
            var builder = PoolManager.Pop<StringBuilder>();
            foreach (var info in md5Info)
            {
                builder.Append(info.ToString("X2"));
            }

            var result = builder.ToString();
            PoolManager.Push(builder);
            builder.Clear();
            return result;
        }

        /// <summary>
        /// 将Excel转化成ScriptableObject
        /// </summary>
        [MenuItem("Tools/JFramework/Update Data", priority = 4)]
        public static void ExcelToScripts()
        {
            var assembly = Reflection.GetAssembly("JFramework.Editor");
            if (assembly == null) return;
            var type = assembly.GetType("JFramework.Editor.ExcelHelper");
            var method = type?.GetMethod("ExcelToScripts", Reflection.Static);
            method?.Invoke(null, null);
        }

        /// <summary>
        /// 当前项目路径
        /// </summary>
        [MenuItem("Tools/JFramework/Project Path", priority = 5)]
        private static void CurrentProjectPath() => Process.Start(Environment.CurrentDirectory);

        /// <summary>
        /// 持久化路径
        /// </summary>
        [MenuItem("Tools/JFramework/PersistentData", priority = 6)]
        private static void PersistentDataPath() => Process.Start(Application.persistentDataPath);

        /// <summary>
        /// 资源流路径
        /// </summary>
        [MenuItem("Tools/JFramework/StreamingAssets", priority = 7)]
        private static void StreamingAssetsPath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
            }

            Process.Start(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 远端加载移除 BuildSetting 反之 加入到 BuildSetting
        /// </summary>
        /// <param name="isRemote"></param>
        public static void AddSceneToBuildSettings(bool isRemote)
        {
            var sceneAssets = EditorBuildSettings.scenes.Select(scene => scene.path).ToList();
            foreach (var scenePath in AssetSetting.Instance.sceneAssets)
            {
                if (sceneAssets.Contains(scenePath))
                {
                    if (!isRemote) continue;
                    var scenes = EditorBuildSettings.scenes.Where(scene => scene.path != scenePath);
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
                else
                {
                    if (isRemote) continue;
                    var scenes = EditorBuildSettings.scenes.ToList();
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }
        }

        /// <summary>
        /// 创建 ScriptableObject 文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Register<T>(string path) where T : ScriptableObject
        {
            var asset = $"{path}/{typeof(T).Name}.asset";
            var instance = AssetDatabase.LoadAssetAtPath<T>(asset);
            if (instance != null) return instance;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            instance = CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, asset);
            AssetDatabase.Refresh();
            return instance;
        }
    }
}
#endif