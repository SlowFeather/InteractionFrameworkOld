using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class KinectModule : BusinessModule
    {

        public GameObject KinectObject;

        public override void Create(object args = null)
        {
            base.Create(args);
            CreateKinect();
        }



        private void CreateKinect()
        {
            KinectObject = GameObject.Instantiate(Resources.Load("Prefabs/KinectManager") as GameObject);
        }
    }
}
