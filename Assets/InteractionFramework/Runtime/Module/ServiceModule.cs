

namespace InteractionFramework.Runtime
{
    /// <summary>
    /// 系统服务模块基类，可以手动释放非托管资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServiceModule<T> : Module, System.IDisposable where T : class, new()
    {
        #region 通用泛型C#单例
        private static T instance = default(T);

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }

        protected void CheckSingleton()
        {
            if (instance == null)
            {
                throw new System.Exception("ServiceModule<" + typeof(T).Name + "> 无法直接实例化，因为它是一个单例!");
            }
        }
        /// <summary>
        ///放非托管资源
        /// </summary>
        public virtual void Dispose()
        {
            UnityEngine.Debug.Log(typeof(T).Name+":Disposed");
            //将该实例从（f-reachable）队列中主动移除
            System.GC.SuppressFinalize(this);
        }

        #endregion
    }
}
