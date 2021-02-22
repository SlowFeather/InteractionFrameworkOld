

using System.Collections.Generic;
using UnityEngine.Events;

namespace InteractionFramework.Runtime
{

    #region 将Unity自带的事件隔离封装自己的事件机制类

    public class ModuleEvent : UnityEvent<object>
    {

    }

    public class ModuleEvent<T> : UnityEvent<T>
    {

    }
    #endregion

    #region 制作事件库 类

    public class EventTable
    {
        private Dictionary<string, ModuleEvent> m_mapEvents=new Dictionary<string, ModuleEvent>();

        public ModuleEvent GetEvent(string type)
        {

            if (!m_mapEvents.ContainsKey(type))
            {

                m_mapEvents.Add(type, new ModuleEvent());
            }

            return m_mapEvents[type];
        }

        public void Clear()
        {
            if (m_mapEvents != null)
            {

                foreach (var @event in m_mapEvents)
                {
                    @event.Value.RemoveAllListeners();
                }
                m_mapEvents.Clear();
            }
        }
    }
    #endregion
}
