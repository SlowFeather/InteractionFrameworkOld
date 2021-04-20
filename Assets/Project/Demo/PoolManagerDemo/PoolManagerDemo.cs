using InteractionFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public class PoolManagerDemo : MonoBehaviour
    {
        SpawnPool testPool;

        PrefabPool testPrefabPool;
        // Start is called before the first frame update
        void Start()
        {
            CreatSpawnPool();
            CreatePrefabPool();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                
                //Debug.Log(PoolManager.Pools["Test"].poolName);
                ///从内存池里面取一个GameObjcet
                Transform momo = PoolManager.Pools["Test"].Spawn("Sphere");
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                //清空池子
                PoolManager.Pools["Test"].DespawnAll();
            }
        }
        /// <summary>
        /// 创建预制体池子
        /// </summary>
        public void CreatePrefabPool()
        {

            if (!testPool._perPrefabPoolOptions.Contains(testPrefabPool))
            {

                testPrefabPool = new PrefabPool(Resources.Load<Transform>("Prefabs/PoolManagerDemo/Sphere"));
                //默认初始化20个Prefab
                testPrefabPool.preloadAmount = 20;
                //如果都选表示缓存池所有的gameobject可以“异步”加载。
                testPrefabPool.preloadTime = false;
                //每几帧加载一个
                testPrefabPool.preloadFrames = 20;
                //延迟多久开始加载，单位是秒。
                testPrefabPool.preloadDelay = 5;
                //开启限制 是否开始实例的限制功能。
                testPrefabPool.limitInstances = true;
                //关闭无限取Prefab 如果我们限制了缓存池里面只能有10个Prefab，如果不勾选它，那么你拿第11个的时候就会返回null。如果勾选它在取第11个的时候他会返回给你前10个里最不常用的那个。
                testPrefabPool.limitFIFO = true;
                //限制池子里最大的Prefab数量 限制缓存池里最大的Prefab的数量，它和上面的preloadAmount是有冲突的，如果同时开启则以limitAmout为准。
                testPrefabPool.limitAmount = 50;
                //开启自动清理池子 是否开启缓存池智能自动清理模式
                testPrefabPool.cullDespawned = true;
                //最终保留 缓存池自动清理，但是始终保留几个对象不清理。
                testPrefabPool.cullAbove = 10;
                //多久清理一次 每过多久执行一遍自动清理，单位是秒。
                testPrefabPool.cullDelay = 30;
                //每次清理几个 每次自动清理几个游戏对象。
                testPrefabPool.cullMaxPerPass = 5;
                //初始化内存池
                testPool.CreatePrefabPool(testPrefabPool);
                testPool._perPrefabPoolOptions.Add(testPrefabPool);
            }
        }
        /// <summary>
        /// 创建SpawnPool
        /// </summary>
        public void CreatSpawnPool()
        {
            //testPool = new SpawnPool("Test");
            //this.gameObject.AddComponent(testPool);
            testPool = this.gameObject.AddComponent<SpawnPool>();
            testPool.poolName = "Test";
            testPool.matchPoolScale = false;
            testPool.matchPoolLayer = false;
            testPool.dontReparent = false;
            testPool.dontDestroyOnLoad = true;
            testPool.MyAwake();
        }

        
    }
}