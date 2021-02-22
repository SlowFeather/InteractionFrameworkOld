# InteractionFramework使用手册

## Editor

### AutoSaveHierarchyScene
将内部`IsAutoSave`设置为`True`可在点击运行按钮时自动保存当前场景
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
初始化分辨率，是否全屏，在哪块屏幕上显示

``` CSharp
configModule.InitResolution();
configModule.InitMonitor();
configModule.InitFrameRate();
```
读取内容位于Ini文件中

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

模型导出FBXhttps://github.com/KellanHiggins/UnityFBXExporter

### UnityMainThreadDispatcher

其他线程访问主线程https://github.com/PimDeWitte/UnityMainThreadDispatcher
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

### FSM （TODO）

https://github.com/thefuntastic/Unity3d-Finite-State-Machine

### Chart （TODO）
https://github.com/spr1ngd/UnityCodes
