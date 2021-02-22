namespace InteractionFramework.Runtime
{
    /// <summary>
    /// 模块基类
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务,如果可能尽量使用如下方法
        /// using(var mc = new MyClass())
        /// {
        /// }
        /// </summary>
        public virtual void Release()
        {
            UnityEngine.Debug.Log("Release");
        }

    }
}
