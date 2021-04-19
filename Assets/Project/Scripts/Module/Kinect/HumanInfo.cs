using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class HumanInfo : MonoBehaviour
    {

        //[Tooltip("要叠加的Kinect关节。")]
        //public KinectInterop.JointType trackedJoint = KinectInterop.JointType.Head;
        ////[Tooltip("用于在背景上覆盖三维对象的照相机。")]
        //private Camera foregroundCamera;

        // Use this for initialization
        void Start()
        {
            //foregroundCamera = Camera.main;
        }

        //private void ReGetPeopleFunction()
        //{
        //    //拿到控件
        //    KinectManager manager = KinectManager.Instance;
        //    //拿到所有人
        //    List<long> alluserid = manager.GetAllUserIds();

        //    //拿到背景框
        //    Rect backgroundRect = foregroundCamera.pixelRect;

        //    //找人身上的某个关节
        //    int iJointIndex = (int)trackedJoint;

        //    if (alluserid.Count > 0)
        //    {
        //        if (manager.IsJointTracked(alluserid[0], iJointIndex))
        //        {
        //            Vector3 posJoint = manager.GetJointPosColorOverlay(alluserid[0], iJointIndex, foregroundCamera, backgroundRect);
        //            MessageManager.Instance.Dispatch(MessageDef.PeoplePos, posJoint);
        //        }
        //        else
        //        {
        //            MessageManager.Instance.Dispatch(MessageDef.NoPeople, null);
        //        }
        //    }

        //    if (alluserid.Count>1)
        //    {
        //        if (manager.IsJointTracked(alluserid[1], iJointIndex))
        //        {
        //            Vector3 posJoint = manager.GetJointPosColorOverlay(alluserid[1], iJointIndex, foregroundCamera, backgroundRect);
        //            MessageManager.Instance.Dispatch(MessageDef.PeopleTwoPos, posJoint);
        //        }
        //        else
        //        {
        //            MessageManager.Instance.Dispatch(MessageDef.NoPeopleTwo, null);
        //        }
        //    }

        //    if (alluserid.Count > 2)
        //    {
        //        if (manager.IsJointTracked(alluserid[2], iJointIndex))
        //        {
        //            Vector3 posJoint = manager.GetJointPosColorOverlay(alluserid[2], iJointIndex, foregroundCamera, backgroundRect);
        //            MessageManager.Instance.Dispatch(MessageDef.PeopleThreePos, posJoint);
        //        }
        //        else
        //        {
        //            MessageManager.Instance.Dispatch(MessageDef.NoPeopleThree, null);
        //        }
        //    }

        //    if (alluserid.Count == 0)
        //    {
        //        MessageManager.Instance.Dispatch(MessageDef.NoPeople, null);
        //        MessageManager.Instance.Dispatch(MessageDef.NoPeopleTwo, null);
        //        MessageManager.Instance.Dispatch(MessageDef.NoPeopleThree, null);


        //    }
        //}



        //void Update()
        //{
        //    ReGetPeopleFunction();
        //}
    }
}