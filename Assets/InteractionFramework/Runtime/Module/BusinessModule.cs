using System.Reflection;
namespace InteractionFramework.Runtime
{
    /// <summary>
    /// 基础模块
    /// </summary>
    public abstract class BusinessModule : Module
    {
        #region 业务模块基类给子类提供常用的字段和属性

        private string m_Name = null;

        public string Name
        {
            get
            {
                if (m_Name == null)
                {
                    m_Name = this.GetType().Name;
                }
                return m_Name;
            }
        }

        public string Title;
        #endregion

        #region 业务模块基类给子类提供的构造和事件机制的使用

        public BusinessModule()
        {

        }

        internal BusinessModule(string name)
        {
            m_Name = name;
        }

        private EventTable m_EventTable;

        public ModuleEvent Event(string type)
        {

            return GetEventTable().GetEvent(type);
        }

        internal void SetEventTable(EventTable mgrEvent)
        {
            m_EventTable = mgrEvent;
        }

        protected EventTable GetEventTable()
        {
            if (m_EventTable == null)
            {
                m_EventTable = new EventTable();
            }
            return m_EventTable;
        }
        #endregion

        #region 业务模块基类给子类提供创建模块和释放模块功能

        public virtual void Create(object args = null)
        {
            UnityEngine.Debug.LogWarning("Create() args = " + args);

        }

        public override void Release()
        {
            if (m_EventTable != null)
            {
                m_EventTable.Clear();
                m_EventTable = null;
            }

            base.Release();
        }
        #endregion

        #region 业务模块基类给子类提供处理消息功能

        internal void HandleMessage(string msg, object[] args)
        {
            UnityEngine.Debug.Log("HandleMessage() msg:"+ msg + ", args:"+ args);

            MethodInfo mi = this.GetType().GetMethod(msg, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null)
            {
                mi.Invoke(this, BindingFlags.NonPublic, null, args, null);
            }
            else
            {

                OnModuleMessage(msg, args);
            }
        }

        protected virtual void OnModuleMessage(string msg, object[] args)
        {
            UnityEngine.Debug.Log("OnModuleMessage() msg:" + msg + ", args:" + args);
        }

        #endregion

        protected virtual void Show(object arg)
        {
            UnityEngine.Debug.Log("Show() arg:"+" arg");
        }

    }
}
