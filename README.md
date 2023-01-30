# 欢迎来到JFramework介绍

1.导入

(1)克隆下来拖进项目进行导入(需要添加AddressableAsset和NewtonsoftJson包)

(2)打开Unity的Package Manager左上角“+”号使用URL方式导入

URL：https://github.com/1176892094/JFramework.git

2.开始

GlobalManager使用：

(1)GlobalManager负责所有管理的初始化

(2)预制体在框架中的Resources文件夹中

(3)将预制体拖入场景即可

ExcelToAsset使用：

(1)在上方工具栏找到Tools/JFramework/ExcelToAsset

(2)或者在JFrameworkInspector中找到ExcelToAsset

(3)找到存放Excel的文件夹，点击确定即可转化成ScriptableObject

(4)配合AddressableAsset工具使用

Addressable使用：

(1)第一次请在上方工具栏找到Window/AssetManagement/Addressable/Groups创建组

(2)在JFrameworkInspector中找到JFrameworkEditor并点击

(3)或者按F1呼出快捷菜单，点击Addressable资源生成

(4)在Addressables Groups面板中可查看资源生成的情况

3.注意

(1)所有的Entity都会被加入到GlobalManager的生命周期中

(2)为了更好的管理，请使用Entity来代替MonoBehaviour

(2)请使用OnUpdate来代替Update

4.使用

(1)EventManager（事件管理类）

```csharp
public class Test1 : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Instance.Listen(EventName.EventTrigger, EventTrigger); //监听事件
    }

    private void Update()
    {
        EventManager.Instance.Send(EventName.EventTrigger); //发送事件
    }

    private void EventTrigger() //触发事件调用该方法
    {
        Debug.Log("触发事件!");
    }

    private void OnDestroy()
    {
        EventManager.Instance.Remove(EventName.EventTrigger, EventTrigger); //移除事件
    }
}

public struct EventName
{
    public const string EventTrigger = "EventTrigger"; //建议定一个事件的常量
}
```
(2)AssetManager（资源加载管理类）
```csharp
public class Test2 : MonoBehaviour
{
    private void LoadAssetAsync() //异步加载
    {
        AssetManager.Instance.LoadAsync<GameObject>(ResPath.Player, obj =>
        {
            Player player = obj.GetComponent<Player>();
        });
    }
}

public struct ResPath
{
    public const string Player = "Prefabs/Player"; //Player预制的体真实路径是：Assets/AddressableResources/Prefabs/Player
}

public class Player: MonoBehaviour
{
}
```
(3)JsonManager（找到Unity上方Tools/JFramework/PersistentPath可查看存档数据）
```csharp
public class Test3 : MonoBehaviour
{
    private void SaveAndLoad1()
    {
        ScriptableObject playerData = ResourceManager.Load<ScriptableObject>(ResPath.PlayerData);
        JsonManager.Instance.Save(playerData, "玩家数据"); //保存SO文件,名称为"玩家数据"
        JsonManager.Instance.Load(playerData); //读取该SO文件
    }

    private void SaveAndLoad2()
    {
        ScriptableObject playerData = ResourceManager.Load<ScriptableObject>(ResPath.PlayerData);
        JsonManager.Instance.Save(playerData, "玩家数据", true); //储存数据并加密
        JsonManager.Instance.Load(playerData, true); //解析加密数据并读取
    }

    private void SaveAndLoad3()
    {
        List<string> playerNameList = new List<string>();
        JsonManager.Instance.Save(playerNameList, "strList"); //储存playerNameList
        playerNameList = JsonManager.Instance.Load<List<string>>("strList"); //读取playerNameList
    }
}
```
(4)PoolManager(对象池工具)
```csharp
public class Test4: MonoBehaviour
{
    private GameObject bullet;
    private async void Start()
    {
        PoolManager.Instance.Pop(PoolPath.Bullet, obj =>
        {
            bullet = obj;//从对象池中取出Bullet
            obj.transform.position = transform.position;//设置生成的子弹位置在自身位置
        });

        await new WaitForSeconds(5);//等待5秒
        PoolManager.Instance.Push(bullet.name, bullet);//将物体放入对象池
    }
}

public struct PoolPath
{
    public const string Bullet = "Bullet";//Bullet的真实路径是：Assets/Resources/Bullet
}
```
(5)AudioManager（游戏声音管理）
```csharp
public class Test5 : MonoBehaviour
{
    private AudioSource audioSource;

    private void BGMusic()
    {
        AudioManager.Instance.PlaySound(AudioPath.BGMusic); //播放背景音乐
        AudioManager.Instance.StopSound(); //停止背景音乐
        AudioManager.Instance.SetSound(0); //改变背景音乐大小为0
    }

    private void GameAudio()
    {
        AudioManager.Instance.PlayAudio(AudioPath.BTClick); //播放该音效
        AudioManager.Instance.PlayAudio(AudioPath.BTClick, audio =>
        {
            audioSource = audio; //播放并获取该音效
        });
        AudioManager.Instance.StopAudio(audioSource); //停止该音效
        AudioManager.Instance.SetAudio(0); //改变游戏音效大小为0
    }
}

public struct AudioPath
{
    public const string BGMusic = "Audio/BGMusic"; //BGMusic的真实路径是：Assets/Resources/Audio/BGMusic
    public const string BTClick = "Audio/BTClick"; //BTClick的真实路径是：Assets/Resources/Audio/BTClick
}
```
(6)UIManager（面板数据请在Resources文件夹中找到名称为UIPanelData的Json文件）
```csharp
public class Test7: MonoBehaviour
{
    private void ShowPanel()
    {
        UIManager.Instance.ShowPanel<LoginPanel>(); //加载LoginPanel(可以重复加载，但只有一个实例)
        UIManager.Instance.ShowPanel<LoginPanel>();//设置层级
        UIManager.Instance.ShowPanel<LoginPanel>(panel =>
        {
            panel.SetUseruame("JINYIJIE");//设置属性
            panel.SetPassword("123456");//设置属性
        });
    }
    
    private void HidePanel()
    {
        UIManager.Instance.HidePanel<LoginPanel>(); //隐藏LoginPane
    }

    private void GetPanel()
    {
        LoginPanel panel = UIManager.Instance.GetPanel<LoginPanel>();//得到面板
        panel.SetUsername("JINYIJIE");//设置属性
        panel.SetPassword("123456");//设置属性
    }

    private void GetLayer()
    {
        UIManager.Instance.GetLayer(UILayerType.Bottom);//得到层级
        Transform common = UIManager.Instance.GetLayer(UILayerType.Height);
    }

    private void Clear()
    {
        UIManager.Instance.Clear();//清除并销毁所有面板
    }
}

public struct UIPanelPath
{
    public const string LoginPanel = "UI/LoginPanel";//LoginPanel的真实路径是：Assets/Resources/UI/LoginPanel
}

public class LoginPanel : UIPanel //需要管理的UI都要继承BasePanel
{
    private string username;
    private string password;
    public void SetUsername(string username) => this.username = username;
    public void SetPassword(string password) => this.password = password;
}
```
(7)LoadManager(场景加载管理)
```csharp
public class Test8 : MonoBehaviour
{
    private float progress; //场景加载进度[0,1]之间

    private void Awake()
    {
        EventManager.Instance.Listen("LoadSceneAsync", LoadSceneAsync); //侦听场景异步加载进度
    }

    private void LoadScene()
    {
        LoadManager.Instance.Load("SceneName");
    }

    private void LoadSceneAsync()
    {
        LoadManager.Instance.LoadAsync("SceneName", () =>
        {
            //异步加载完成后执行
        });
    }

    private void LoadSceneAsync(params object[] args) //获取异步加载进度
    {
        progress = (float)args[0]; //获取当前加载进度
        Debug.Log(progress);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Remove("LoadSceneAsync", LoadSceneAsync); //移除场景异步加载进度
    }
}
```
(8)TimerManager(自定义计时器工具)
```csharp
public class Test9 : MonoBehaviour
{
    private TimeTick timer;

    private void Start()
    {
        timer = TimerManager.Instance.Listen(5, () =>
        {
            Debug.Log("不循环/间隔5秒的计时器完成");
        });

        TimerManager.Instance.Listen(5, () =>
        {
            Debug.Log("不循环/间隔5秒的不受TimeScale影响的计时器完成");
        }).Unscale();

        int count = 0;
        TimerManager.Instance.Listen(1, () =>
        {
            Debug.Log("循环5次/间隔1秒的计时器完成");
        }).Unscale().SetLoop(5, () =>
        {
            count++;
            Debug.Log("第" + count + "次循环完成");
        });
        
        TimerManager.Instance.Listen(10, () =>
        {
            Debug.Log("设置计时器随物体销毁而停止");
        }).SetTarget(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) timer.Stop();  //启动计时器，从剩余时间开始
        if (Input.GetKeyDown(KeyCode.E)) timer.Play();  //暂停计时器，从当前时间停止
    }
}
```
(9)StateMachine(有限状态机)
```csharp
    public class Enemy : Machine //敌人继承状态机
    {
        public Animator an; //敌人动画组件

        protected override void Awake()
        {
            base.Awake();
            ListenState("Idle", new EnemyIdle()); //状态机增加Idle状态
            ListenState("Walk", new EnemyWalk()); //状态机增加Walk状态
        }
    }

    public class EnemyIdle : State<Enemy> //设置状态的所有者
    {
        protected override void OnInit(Enemy owner)
        {
            base.OnInit(owner); //父类初始化状态
            //自定义初始化
        }

        protected override void OnEnter()
        {
            owner.an.SetBool("Idle",true); //播放Idle动画
            owner.ChangeState("Walk",3); //3秒切换到Walk动画
        }

        protected override void OnUpdate() //状态更新
        {
        }

        protected override void OnExit()
        {
            owner.an.SetBool("Idle",false); //停止Idle动画
        }
    }

    public class EnemyWalk : State<Enemy> //设置状态的所有者
    {
        protected override void OnEnter()
        {
            owner.an.SetBool("Walk",true); //播放Walk动画
            owner.ChangeState("Idle",3); //3秒切换到Idle动画
        }

        protected override void OnUpdate()
        {
           
        }

        protected override void OnExit()
        {
            owner.an.SetBool("Walk",false); //停止Walk动画
        }
    }
```
(10)Entity/Controller(EC实体控制器分离)
```csharp
    public class Player : Entity //玩家继承实体
    {
        public BuffController buffCtrl; //效果控制器
        public SkillController skillCtrl; //技能控制器
        public AttributeController attrCtrl; //属性控制器

        protected override void Awake()
        {
            base.Awake();
            skillCtrl = ScriptableObject.CreateInstance<SkillController>();
            buffCtrl = ScriptableObject.CreateInstance<BuffController>();
            attrCtrl = ScriptableObject.CreateInstance<AttributeController>();
            skillCtrl.Get<IController>().OnInit(this);
            attrCtrl.Get<IController>().OnInit(this);
            buffCtrl.Get<IController>().OnInit(this);
        }
    }

    public class SkillController : Controller<Player> //技能控制器(ScriptableObject)
    {
        private AttributeController attrCtrl => owner.attrCtrl;
    }

    public class AttributeController : Controller<Player> //属性控制器(ScriptableObject)
    {
        private SkillController skillCtrl => owner.skillCtrl;
    }

    public class BuffController : Controller<Player> //效果控制器(ScriptableObject)
    {
        private AttributeController attrCtrl => owner.attrCtrl;
        private SkillController skillCtrl => owner.skillCtrl;
    }
```

(11)AwaitExtensions(异步拓展)
```csharp
  private async void Start()
    {
        await new WaitForSeconds(1);//等待一秒
        await new WaitForSecondsRealtime(1);//等待1秒，不受timeScale影响
        await new WaitForUpdate();//在Update最后一帧执行
        await new WaitForFixedUpdate();//在FixedUpdate最后一帧执行
        await new WaitForEndOfFrame();//等待这一帧结束
        await new WaitWhile(WaitTime);//等待WaitTime结果，不会挂起异步
        await new WaitUntil(WaitTime);//等待WaitTime结果，false不会执行后面语句
        await SceneManager.LoadSceneAsync("SceneName");
        await Resources.LoadAsync("ResourcesName");
        AsyncOperation asyncOperation = new AsyncOperation();
        await asyncOperation;//等待异步操作
        ResourceRequest request = new ResourceRequest();
        await request;//等待资源请求
        AssetBundleRequest bundleRequest = new AssetBundleRequest();
        await bundleRequest;//等待AB包请求
        AssetBundleCreateRequest bundleCreateRequest = new AssetBundleCreateRequest();
        await bundleCreateRequest;//等待AB包创建请求
    }
    
    private bool WaitTime()
    {
        return true;
    }
```
