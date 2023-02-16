using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JFramework
{
    public class LoadManager : Singleton<LoadManager>
    {
        /// <summary>
        /// 当前场景名称
        /// </summary>
        public string Scene => SceneManager.GetActiveScene().name;

        /// <summary>
        /// 场景列表
        /// </summary>
        internal List<SceneData> sceneList;

        /// <summary>
        /// 场景管理器初始化
        /// </summary>
        public override void Awake()
        {
            sceneList = new List<SceneData>();
            var count = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < count; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                var array = path.Split('/');
                var name = array[^1];
                array = name.Split('.');
                name = array[0];
                sceneList.Add(new SceneData() { Name = name, Id = i });
            }
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="name">场景名称</param>
        public void LoadScene(string name)
        {
            if (sceneList == null)
            {
                Debug.Log("场景管理器没有初始化!");
                return;
            }

            SceneManager.LoadScene(name);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="action">场景加载完成的回调</param>
        public void LoadSceneAsync(string name, Action action)
        {
            if (sceneList == null)
            {
                Debug.Log("场景管理器没有初始化!");
                return;
            }

            GlobalManager.Instance.StartCoroutine(LoadSceneCompleted(name, action));
        }

        /// <summary>
        /// 通过携程异步加载场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="action">场景加载完成的回调</param>
        /// <returns>返回场景加载迭代器</returns>
        private IEnumerator LoadSceneCompleted(string name, Action action)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                float progress = 0;
                while (progress < 0.999f)
                {
                    progress = Mathf.Lerp(progress, asyncOperation.progress / 9f * 10f, Time.deltaTime);
                    EventManager.Instance.Send(999, progress);
                    yield return new WaitForEndOfFrame();
                }

                asyncOperation.allowSceneActivation = true;
                yield return null;
            }

            action?.Invoke();
            Debug.Log($"场景管理器异步加载 {name} 场景完成!");
        }
    }
}