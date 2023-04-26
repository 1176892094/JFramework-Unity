using System.Collections.Generic;
using JFramework.Interface;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

// ReSharper disable All
namespace JFramework
{
    public class EntitySpecial : Entity
    {
        /// <summary>
        /// 控制器容器
        /// </summary>
        [ShowInInspector, LabelText("控制器容器"), SerializeField]
        private readonly Dictionary<string, IController> controllerDict = new Dictionary<string, IController>();

        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <typeparam name="T">可使用任何继承IController的对象</typeparam>
        /// <returns>返回控制器对象</returns>
        public T Get<T>() where T : ScriptableObject, IController
        {
            var key = typeof(T).Name;
            if (controllerDict.ContainsKey(key)) return (T)controllerDict[key];
            var controller = ScriptableObject.CreateInstance<T>();
            controllerDict.Add(key, controller);
            controller.Spawn(this);
            return controller;
        }

        /// <summary>
        /// 实体销毁
        /// </summary>
        public override void Despawn()
        {
            controllerDict?.Values.ForEach(controller => controller.Despawn());
        }
    }
}