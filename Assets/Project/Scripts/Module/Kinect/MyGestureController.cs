using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime {
    public class MyGestureController : MonoBehaviour, KinectGestures.GestureListenerInterface
    {
        //public int playerIndex = 0;

        //public long userid=0;

        private List<KinectGestures.Gestures> KinectGesturesGesturesList=new List<KinectGestures.Gestures>();

        public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
        {
            return true;
        }

        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {
            Debug.Log("GestureCompleted" + " id:" + userId + " index:" + userIndex + " gesture:" + gesture.ToString()+ " joint:" + joint.ToString()+ " screenpos:" + screenPos.ToString());
            //if (userIndex != playerIndex)
            //    return false;
            //if (userid != userId)
            //{
            //    return false;
            //}
            //if (gesture == KinectGestures.Gestures.SwipeLeft)
            //{
            //    //Debug.Log("SwipeLeft");
            //    //MessageManager.Instance.Dispatch(MessageDef.ThirdScene_SwipeLeft, null);
            //}
            return true;
        }

        public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {

        }

        public void UserDetected(long userId, int userIndex)
        {
            KinectManager manager = KinectManager.Instance;//初始化KinectManager对象
            for (int i = 0; i < KinectGesturesGesturesList.Count; i++)
            {
                manager.DetectGesture(userId, KinectGesturesGesturesList[i]);//添加姿势检测
            }
        }

        public void UserLost(long userId, int userIndex)
        {
            KinectManager manager = KinectManager.Instance;//初始化KinectManager对象
            for (int i = 0; i < KinectGesturesGesturesList.Count; i++)
            {
                manager.DeleteGesture(userId, KinectGesturesGesturesList[i]);//添加姿势检测
            }
        }

    }
}