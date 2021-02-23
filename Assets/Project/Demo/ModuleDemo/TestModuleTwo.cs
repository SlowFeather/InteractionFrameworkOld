using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class TestModuleTwo : BusinessModule
    {
        /// <summary>
        /// 创建Module时触发
        /// </summary>
        /// <param name="args"></param>
        public override void Create(object args = null)
        {
            base.Create(args);
            Debug.Log("Create TestModuleTwo Module");
        }
        /// <summary>
        /// 当收到消息时触发
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        protected override void OnModuleMessage(string msg, object[] args)
        {
            base.OnModuleMessage(msg, args);
            Debug.Log("Get Message : " + msg + " args : " + args[0].ToString());
        }
    }
}