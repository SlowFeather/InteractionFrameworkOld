# InteractionFramework使用手册
## 概念
全模块化管理，无论是摄像头调用、二维码生成/识别、Kinect实时获取人物位置、多块屏幕同时显示/多块屏幕显示不同内容、雷达获取触点位置都是单独的模块，可随时添加或者删除。
(以上均为未来实现功能)
## Editor
### SaveHierarchyScene
将内部`IsSave`设置为`True`可在点击运行按钮时自动保存当前场景所做的改动
### Inspector
**Inspector**面板扩展
- 快速将`Transform`归零
- 一键复制粘贴`Transform`中的值
### ToolBar
1. 工具栏相关,用于打开以下文件夹

- `Application.dataPath` 
- `Application.persistentDataPath` 
- `Application.streamingAssetsPath` 
- `Application.temporaryCachePath`

2. 将当前场景或者所有场景添加到`BuildSetting`
3. 将Debug工具添加到*Hierarchy*
4. 可视化管理所有的PlayerPrefs
5. 可视化管理所有宏定义
## Runtime

### Singleton
#### Singleton
Class继承Singleton
``` CSharp
   SingletionOne.Instance.TestFunction();
```
#### MonoSingleton
Class继承MonoSingleton
``` CSharp
   //初始化
   MonoSingletionOne.InitSingletion();
   //检查
   MonoSingletionOne.CheckInstance();
   //调用
   MonoSingletionOne.Instance.TestFunction();
```

### Module

#### 创建Module
``` CSharp
namespace InteractionFramework.Runtime
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
```
#### Module管理器
``` CSharp
//使用命名空间来初始化Module
string namespacepath = "InteractionFramework.Runtime.Demo";
ModuleManager.Instance.Init(namespacepath);
//使用Module名称来初始化模块
ModuleManager.Instance.CreateModule("TestModuleOne");
//给某个Module发消息
ModuleManager.Instance.SendMessage("TestModuleOne", "HelloOne", "MsgContent");
//得到某个Module
TestModuleOne module1 = ModuleManager.Instance.GetModule<TestModuleOne>();
TestModuleOne module2 = ModuleManager.Instance.GetModule("TestModuleOne") as TestModuleOne;
```

## Project
### Manager
#### UIManager

``` CSharp
//检查单例
UIManager.InitSingletion();
UIManager.CheckInstance();
//设置预制体路径
UIManager.Instance.uiPath = "Prefabs/UIDemo/";
//找到并设置UI根物体
GameObject uiRoot = GameObject.Find("Canvas");
//初始化
UIManager.Instance.InitUIManager(uiRoot);


//打开界面
UIManager.Instance.OpenPage(TestUIDef.TestUI);
//界面方法
UIManager.Instance.GetPage<TestUI>(TestUIDef.TestUI).Hello();
//关闭界面
UIManager.Instance.ClosePage(TestUIDef.TestUI);
//移除界面
UIManager.Instance.RemovePage(TestUIDef.TestUI);
```
子页面需要继承```UIPage```
``` CSharp
public class TestUI : UIPage
{
    //生成时触发一次
    protected override void OnInitialize()
    {
        base.OnInitialize();
        Debug.Log("Test UI OnInitialize");
    }
    //每次打开时触发
    protected override void OnOpen(object arg)
    {
        base.OnOpen(arg);
        Debug.Log("Test UI OnOpen");

    }
    //每次关闭时触发
    protected override void OnClose()
    {
        base.OnClose();
        Debug.Log("Test UI OnClose");
    }

    //自定义方法
    public void Hello()
    {
        Debug.Log("Hello");
    }
}
```
#### PoolManagerHelper
对象池辅助工具
创建基础池子```CreatSpawnPool()```

创建预制体池子（需要有基础池子）```CreatePrefabPool()```
``` CSharp
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
```

### Module
#### CameraModule
CameraModule为服务模块，所以是单例形式

开启摄像头
``` CSharp
CameraModule.Instance.InitDevice(0,640,480);
```
关闭摄像头
``` CSharp
CameraModule.Instance.StopDevice();
```
开启摄像头之后可以使用```CameraModule.Instance.webCamTexture```来拿到摄像头画面
#### QRCodeModule
使用基础模块之前一般会进行初始化模块管理器
``` CSharp
//初始化模块管理器
string namespacepath = "InteractionFramework.Runtime";
ModuleManager.Instance.Init(namespacepath);
```
QRCodeHelper中使用了多线程，所以需要初始化主线程访问工具
``` CSharp
//初始化多线程跨线程访问主线程工具
UnityMainThreadDispatcher.InitSingletion();
```
之后需要启动摄像头

``` CSharp
 //启动摄像头
CameraModule.Instance.InitDevice(0,640,480);

if (CameraModule.Instance.webCamTexture!=null)
{
    //拿到摄像头的图像
    camTexture.texture = CameraModule.Instance.webCamTexture;
}
```
拿到二维码模块，然后添加监听，生成即可
``` CSharp
//创建生成二维码工具模块
qRCodeModule =(QRCodeModule)ModuleManager.Instance.CreateModule(ModuleDef.QRCodeModule);
//添加生成结束监听
qRCodeModule.CreateQRCodedEvent += CreateQRCodedEventHandler;
//开始生成
qRCodeModule.CreateQRCode();
```
监听方法
``` CSharp
/// <summary>
/// 二维码生成完毕监听 
/// </summary>
/// <param name="texture2d"></param>
private void CreateQRCodedEventHandler(Texture2D texture2d)
{
    qRCodeModule.CreateQRCodedEvent -= CreateQRCodedEventHandler;
    qrCodeTexture.texture = texture2d;
}
```

#### ConfigModule
读取设置文件中的内容，完成设置（部分需要重启生效）
``` CSharp
string namespacepath = "InteractionFramework.Runtime";
ModuleManager.Instance.Init(namespacepath);
//创建生成二维码工具模块
ConfigModule configModule=(ConfigModuModuleManager.Instance.CreateModule(ModuleDConfigModule);
// 初始化分辨率
configModule.InitResolution();
// 初始化播放屏幕
// 设置完屏幕需要重启生效
configModule.InitMonitor();
// 设置帧率
configModule.InitFrameRate();
```

#### SafetyLockModule

``` CSharp
//初始化模块命名空间
string namespacepath = "InteractionFramework.Runtime";
ModuleManager.Instance.Init(namespacepath);
//初始化ini读写模块
IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";
//创建模块
safetyLockModule = (SafetyLockModule)ModuleManager.Instance.CreateModule(ModuleDef.SafetyLockModule);

//设置一天过期 0则永不过期
//safetyLockModule.availableTime = 0;
safetyLockModule.availableTime = 1;
//添加过没过期的监听
safetyLockModule.ExpireEvent += ExpireEventHandler;
//初始化
safetyLockModule.Init();
```
回调
``` CSharp
private void ExpireEventHandler(bool isExpire)
{
    if (isExpire)
    {
        Debug.Log("过期了");
    }
    else
    {
        Debug.Log("没过期");
    }
}
```


## Plugins
### INI
用于运行时读取或写入设置文件

https://blog.csdn.net/le_sam/article/details/78416052

如果需要在运行时写入，将IniStorage中的mINIFileName修改，改为沙盒路径下即可运行时读取写入
``` CSharp
//不需要运行时修改，放入StreamAssets下
public static string mINIFileName = Application.streamingAssetsPath + "/config.ini";
//需要运行时修改，则放入沙盒路径下
//public static string mINIFileName = Application.persistentDataPath + "/config.ini";
```
### xNode
用于绘制连线
https://github.com/Siccity/xNode

### AudioManager
用于音频播放

https://github.com/oluwaseyeayinla/Papae.AudioManager

### IngameDebugConsole
用于Debug

https://github.com/yasirkula/UnityIngameDebugConsole

### NaughtyAttributes
用于自定义Inspector

https://github.com/dbrizov/NaughtyAttributes

### PlayerPrefsEditor
可视化管理所有的PlayerPrefs

https://github.com/Dysman/bgTools-playerPrefsEditor

### DefineInspector
可视化管理所有宏定义

https://github.com/haydenjameslee/unity-define-inspector

### UnityFBXExporter

模型导出FBX

https://github.com/KellanHiggins/UnityFBXExporter

### UnityMainThreadDispatcher

其他线程访问主线程

https://github.com/PimDeWitte/UnityMainThreadDispatcher
``` CSharp
public IEnumerator ThisWillBeExecutedOnTheMainThread()
{
   Debug.Log("This is executed from the main thread");
   this.transform.position = Vector3.one * 20;
   yield return null;
}
public void ExampleMainThreadCall()
{
   //访问主线程
   UnityMainThreadDispatcher.Instance.Enqueue(ThisWillBeExecutedOnTheMainThread());
}
```
或者直接按照以下方式
``` CSharp
UnityMainThreadDispatcher.Instance.Enqueue(() => {
   Debug.Log("This is executed from the main thread");
   this.transform.position = Vector3.one * i;
});
```
### Timer
定时器
https://github.com/akbiggs/UnityTimer
``` C#
Timer.Register(5f, () => Debug.Log("Hello World"));
```
通过`Timer.Cancel`取消计时器
``` C#
Timer timer;

void Start() {
   timer = Timer.Register(2f, () => Debug.Log("You won't see this text if you press X."));
}

void Update() {
   if (Input.GetKeyDown(KeyCode.X)) {
      Timer.Cancel(timer);
   }
}
```
通过设置isLooped为true来重复计时器。
``` CSharp
// Let's say you pause your game by setting the timescale to 0.
// 假设您通过将时间刻度设置为0来暂停游戏。
Time.timeScale = 0f;

// ...Then set useRealTime so this timer will still fire even though the game time isn't progressing.
//…然后设置useRealTime，这样即使游戏时间没有进展，计时器仍会启动。
Timer.Register(1f, this.HandlePausedGameState, useRealTime: true);
```
随GameObject销毁而自动取消的计时器
``` CSharp
public class CoolMonoBehaviour : MonoBehaviour {

   void Start() {
      // Use the AttachTimer extension method to create a timer that is destroyed when this
      // object is destroyed.
      this.AttachTimer(5f, () => {
      
         // If this code runs after the object is destroyed, a null reference will be thrown,
         // which could corrupt game state.
         this.gameObject.transform.position = Vector3.zero;
      });
   }
   
   void Update() {
      // This code could destroy the object at any time!
      if (Input.GetKeyDown(KeyCode.X)) {
         GameObject.Destroy(this.gameObject);
      }
   }
}
```
使用`onUpdate`回调随着时间的推移逐渐更新值。

``` CSharp
// Change a color from white to red over the course of five seconds.
//在五秒钟内将颜色从白色更改为红色。
Color color = Color.white;
float transitionDuration = 5f;

Timer.Register(transitionDuration,
   onUpdate: secondsElapsed => color.r = 255 * (secondsElapsed / transitionDuration),
   onComplete: () => Debug.Log("Color is now red"));
```
包括许多其他有用的功能
- timer.Pause（）
- timer.Resume（）
- timer.GetTimeRemaining（）
- timer.GetRatioComplete（）
- timer.isDone

更改场景时，所有计时器均被销毁。通常，这种行为是需要的，并且发生是因为计时器由TimerController更新，并且在场景更改时销毁了该Co​​ntroller。请注意，其结果是，在关闭场景时（例如，在对象的OnDestroy方法中）创建Timer会在生成TimerController时导致Unity错误。

### FSM
状态机StateMachine https://github.com/thefuntastic/Unity3d-Finite-State-Machine
``` CSharp
using MonsterLove.StateMachine; //1. Remember the using statement

public class MyGameplayScript : MonoBehaviour
{
    public enum States
    {
        Init, 
        Play, 
        Win, 
        Lose
    }
    
    StateMachine<States> fsm;
    
    void Awake(){
        fsm = new StateMachine<States>(this); //2. The main bit of "magic". 

        fsm.ChangeState(States.Init); //3. Easily trigger state transitions
    }

    void Init_Enter()
    {
        Debug.Log("Ready");
    }

    void Play_Enter()
    {      
        Debug.Log("Spawning Player");    
    }

    void Play_FixedUpdate()
    {
        Debug.Log("Doing Physics stuff");
    }

    void Play_Update()
    {
        if(player.health <= 0)
        {
            fsm.ChangeState(States.Lose); //3. Easily trigger state transitions
        }
    }

    void Play_Exit()
    {
        Debug.Log("Despawning Player");    
    }

    void Win_Enter()
    {
        Debug.Log("Game Over - you won!");
    }

    void Lose_Enter()
    {
        Debug.Log("Game Over - you lost!");
    }

}
```
### PoolManager
更多详见```PoolManagerHelper```
``` CSharp
//从池子里面取一个GameObjcet，并且设置他的父物体
Transform momo = PoolManager.Pools["Test"].Spawn("Sphere",this.transform);

//将此物体回收，重置他的父物体
PoolManager.Pools["Test"].Despawn(this.transform, PoolManager.Pools["Pool"].transform);

//清空池子
PoolManager.Pools["Test"].DespawnAll();
```

### Chart （TODO）
https://github.com/spr1ngd/UnityCodes
