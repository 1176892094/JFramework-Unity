using JFramework.Interface;
using Sirenix.OdinInspector;

namespace JFramework
{
    /// <summary>
    /// 控制器的抽象类
    /// </summary>
    /// <typeparam name="T">实现了IEntity接口的类型</typeparam>
    public abstract class Controller<T> : SerializedScriptableObject, IController where T : IEntity
    {
        /// <summary>
        /// 控制器的所有者
        /// </summary>
        public T owner;

        /// <summary>
        /// 控制器初始化
        /// </summary>
        /// <param name="owner">传入控制器的所有者</param>
        private void OnInit(T owner)
        {
            this.owner = owner;
            OnInit();
        }

        /// <summary>
        /// 控制器初始化
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 通过接口初始化控制器
        /// </summary>
        /// <param name="owner">控制器的所有者</param>
        void IController.OnInit(object owner) => OnInit((T)owner);
    }
}