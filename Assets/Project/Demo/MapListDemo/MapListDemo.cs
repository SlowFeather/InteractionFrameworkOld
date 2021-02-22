using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class MapListDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MapList<int, string> mapList = new MapList<int, string>();

            mapList.Add(0, "A");
            mapList.Add(1, "B");
            mapList.Add(2, "C");
            mapList.Add(3, "D");
            mapList.Add(4, "E");

            Debug.Log("Value : " + mapList[0]);
            Debug.Log("Value : " + mapList[1]);
            Debug.Log("Value : " + mapList[2]);
            Debug.Log("Value : " + mapList[3]);
            Debug.Log("Value : " + mapList[4]);

            mapList.Remove(4);

            Debug.Log("Value : " + mapList[0]);
            Debug.Log("Value : " + mapList[1]);
            Debug.Log("Value : " + mapList[2]);
            Debug.Log("Value : " + mapList[3]);
            Debug.Log("Value : " + mapList[4]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}