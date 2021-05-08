using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class PoolManagerHelper : MonoSingletion<PoolManagerHelper>
    {
        public SpawnPool Pools;
        private void Start()
        {
            CreatSpawnPool(this.gameObject);
        }

        /// <summary>
        /// 创建SpawnPool
        /// </summary>
        public void CreatSpawnPool(GameObject poolParent)
        {
            SpawnPool myPool = poolParent.AddComponent<SpawnPool>();
            myPool.poolName = "Pool";
            myPool.matchPoolScale = false;
            myPool.matchPoolLayer = false;
            myPool.dontReparent = false;
            myPool.dontDestroyOnLoad = true;

            Pools = myPool;
        }

        /// <summary>
        /// 创建预制体池子
        /// </summary>
        public void CreatePrefabPool(string prefabPath,int maxNumber)
        {
            SpawnPool myPool = PoolManager.Pools["Pool"];
            PrefabPool myPrefabPool = new PrefabPool(Resources.Load<Transform>(prefabPath));

            if (myPool._perPrefabPoolOptions.Contains(myPrefabPool))
            {
                Debug.LogError("Has Pool!!!");
                return;
            }

            //默认初始化20个Prefab
            myPrefabPool.preloadAmount = maxNumber;
            //如果都选表示缓存池所有的gameobject可以“异步”加载。
            myPrefabPool.preloadTime = false;
            //每几帧加载一个
            myPrefabPool.preloadFrames = 20;
            //延迟多久开始加载，单位是秒。
            myPrefabPool.preloadDelay = 5;
            //开启限制 是否开始实例的限制功能。
            myPrefabPool.limitInstances = true;
            //关闭无限取Prefab 如果我们限制了缓存池里面只能有10个Prefab，如果不勾选它，那么你拿第11个的时候就会返回null。如果勾选它在取第11个的时候他会返回给你前10个里最不常用的那个。
            myPrefabPool.limitFIFO = true;
            //限制池子里最大的Prefab数量 限制缓存池里最大的Prefab的数量，它和上面的preloadAmount是有冲突的，如果同时开启则以limitAmout为准。
            myPrefabPool.limitAmount = maxNumber;
            //开启自动清理池子 是否开启缓存池智能自动清理模式
            myPrefabPool.cullDespawned = false;
            //最终保留 缓存池自动清理，但是始终保留几个对象不清理。
            myPrefabPool.cullAbove = 10;
            //多久清理一次 每过多久执行一遍自动清理，单位是秒。
            myPrefabPool.cullDelay = 30;
            //每次清理几个 每次自动清理几个游戏对象。
            myPrefabPool.cullMaxPerPass = 5;
            //初始化内存池
            myPool.CreatePrefabPool(myPrefabPool);
            myPool._perPrefabPoolOptions.Add(myPrefabPool);
        }
    }
}