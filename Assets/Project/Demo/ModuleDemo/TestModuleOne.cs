using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class TestModuleOne : BusinessModule
    {
        public override void Create(object args = null)
        {
            base.Create(args);
            Debug.Log("Create TestModuleOne Module");
        }

        protected override void OnModuleMessage(string msg, object[] args)
        {
            base.OnModuleMessage(msg, args);
            Debug.Log("Get Message : " + msg + " args : " + args[0].ToString());
        }

    }
}