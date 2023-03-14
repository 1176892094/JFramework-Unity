using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JFramework
{
    /// <summary>
    /// 单例模式对象
    /// </summary>
    /// <typeparam name="T">所属的单例对象</typeparam>
    public abstract class DataSingleton<T> : ScriptableObject where T : DataSingleton<T>
    {
        /// <summary>
        /// 所属单例对象
        /// </summary>
        private static T instance;
        
        /// <summary>
        /// 返回单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
#if UNITY_EDITOR
                var name = $"Assets/Editor/{typeof(T).Name}.asset";
                instance = AssetDatabase.LoadAssetAtPath<T>(name);
                if (instance != null) return instance;
                instance = CreateInstance<T>();
                if (!Directory.Exists("Assets/Editor"))
                {
                    Directory.CreateDirectory("Assets/Editor");
                }

                AssetDatabase.CreateAsset(instance, name);
                AssetDatabase.Refresh();
#else
                instance = AssetManager.Instance.Load<T>("Setttngs/"+typeof(T).Name);
#endif
                return instance;
            }
        }
    }
}