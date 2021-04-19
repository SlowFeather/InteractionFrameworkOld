using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class UIModule : BusinessModule
    {
        UIRoot uiRoot;

        public override void Create(object args = null)
        {
            base.Create(args);
            uiRoot = (UIRoot)args;
        }

        public void OpenPage()
        {

        }
        public void JumpPage()
        {

        }

        public void ClosePage()
        {

        }

        
    }
}