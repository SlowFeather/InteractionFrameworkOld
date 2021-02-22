

using System;
using System.Collections.Generic;
namespace InteractionFramework.Runtime
{

    public class ModuleManager : ServiceModule<ModuleManager>
    {

        #region 对已创建模块 和未创建模块的事件预监听以及未创建模块消息缓存的  容器字段实例化

        private Dictionary<string, BusinessModule> m_BusinessModules;

        private Dictionary<string, EventTable> m_PreListenEvents;
        class MessageObject
        {
            public string target;
            public string msg;
            public object[] args;
        }

        private Dictionary<string, List<MessageObject>> m_CacheMessages;

        private string m_ModuleDomain;

        public ModuleManager()
        {
            m_BusinessModules = new Dictionary<string, BusinessModule>();
            m_CacheMessages = new Dictionary<string, List<MessageObject>>();
            m_PreListenEvents = new Dictionary<string, EventTable>();

        }

        public void Init(string domain = "")
        {
            CheckSingleton();
            m_ModuleDomain = domain;
        }
        #endregion

        #region 模块管理器提供 根据子类业务类型创建业务模块 功能

        private T CreateModule<T>(object args = null) where T : BusinessModule
        {
            return (T)CreateModule(typeof(T).Name, args);
        }

        public BusinessModule CreateModule(string name, object args = null)
        {
            UnityEngine.Debug.LogWarning("CreateModule() name = " + name + ", args = " + args);

            if (m_BusinessModules.ContainsKey(name))
            {
                UnityEngine.Debug.LogError("CreateModule() The Module " + name + " Has Existed!");

                return null;
            }

            BusinessModule module = null;
            
            Type type = Type.GetType(m_ModuleDomain + "." + name);
            if (type != null)
            {
                module = Activator.CreateInstance(type) as BusinessModule;
            }
            else
            {
                //module = new LuaModule(name);
                UnityEngine.Debug.LogError("CreateModule() The Module "+ name+" Is LuaModule!");
                return null;
            }
            m_BusinessModules.Add(name, module);

            if (m_PreListenEvents.ContainsKey(name))
            {
                EventTable mgrEvent = null;

                mgrEvent = m_PreListenEvents[name];
                m_PreListenEvents.Remove(name);

                module.SetEventTable(mgrEvent);
            }

            module.Create(args);

            if (m_CacheMessages.ContainsKey(name))
            {
                List<MessageObject> list = m_CacheMessages[name];
                for (int i = 0; i < list.Count; i++)
                {
                    MessageObject msgobj = list[i];
                    module.HandleMessage(msgobj.msg, msgobj.args);
                }
                m_CacheMessages.Remove(name);
            }

            return module;
        }
        #endregion

        #region 模块管理器提供 释放由ModuleManager创建的模块  功能

        public void ReleaseModule(BusinessModule module)
        {
            if (module != null)
            {
                if (m_BusinessModules.ContainsKey(module.Name))
                {
                    UnityEngine.Debug.Log("ReleaseModule() name = " + module.Name);
                    m_BusinessModules.Remove(module.Name);
                    module.Release();
                }
                else
                {
                    UnityEngine.Debug.LogError("ReleaseModule() 模块不是由ModuleManager创建的！ name = " + module.Name);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("ReleaseModule() module = null!");
            }

        }

        public void ReleaseAll()
        {
            foreach (var @event in m_PreListenEvents)
            {
                @event.Value.Clear();
            }
            m_PreListenEvents.Clear();

            m_CacheMessages.Clear();

            foreach (var module in m_BusinessModules)
            {
                module.Value.Release();
            }
            m_BusinessModules.Clear();
        }
        #endregion

        #region 模块管理器提供  获取一个模块Module ,如果未创建过该Module，则返回null  功能
        public T GetModule<T>() where T : BusinessModule
        {
            return GetModule(typeof(T).Name) as T;
        }

        public BusinessModule GetModule(string name)
        {
            if (m_BusinessModules.ContainsKey(name))
            {
                return m_BusinessModules[name];
            }
            return null;
        }
        #endregion

        public void ShowModule(string name, object arg = null)
        {
			SendMessage(name, "Show", arg);
        }

        #region 模块管理器提供  模块之间通讯的消息机制  功能

        public void SendMessage(string target, string msg, params object[] args)
        {
            SendMessage_Internal(target, msg, args);
        }

        private void SendMessage_Internal(string target, string msg, object[] args)
        {

            BusinessModule module = GetModule(target);
            if (module != null)
            {
                module.HandleMessage(msg, args);
            }
            else
            {

                List<MessageObject> list = GetCacheMessageList(target);
                MessageObject obj = new MessageObject();
                obj.target = target;
                obj.msg = msg;
                obj.args = args;
                list.Add(obj);

                UnityEngine.Debug.LogWarning("SendMessage() target不存在！将消息缓存起来! target:"+ target+", msg:"+ msg+", args:{2}"+args);
            }
        }

        private List<MessageObject> GetCacheMessageList(string target)
        {
            List<MessageObject> list = null;

            if (!m_CacheMessages.ContainsKey(target))
            {
                list = new List<MessageObject>();
                m_CacheMessages.Add(target, list);
            }
            else
            {
                list = m_CacheMessages[target];
            }
            return list;
        }
        #endregion

        #region 模块管理器提供  模块之间通讯的事件监听机制  功能

        public ModuleEvent Event(string target, string type)
        {
            ModuleEvent evt = null;
            BusinessModule module = GetModule(target);
            if (module != null)
            {
                evt = module.Event(type);
            }
            else
            {

                EventTable table = GetPreEventTable(target);
                evt = table.GetEvent(type);
                UnityEngine.Debug.LogWarning("Event() target不存在！将预监听事件! target:"+target+", event:"+ type);
            }
            return evt;
        }

        private EventTable GetPreEventTable(string target)
        {
            EventTable table = null;
            if (!m_PreListenEvents.ContainsKey(target))
            {
                table = new EventTable();
                m_PreListenEvents.Add(target, table);
            }
            else
            {
                table = m_PreListenEvents[target];
            }
            return table;
        }
        #endregion

    }
}
