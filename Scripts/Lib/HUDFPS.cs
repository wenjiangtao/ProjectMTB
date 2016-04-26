using UnityEngine;
using System.Collections;
using MTB;
public class HUDFPS : MonoBehaviour
{

    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    public float updateInterval = 0.5F;

    private World world;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float fps = 0;
    private GameObject editorUI;

    private bool isEditorMode;

    void Start()
    {
        timeleft = updateInterval;
        world = World.world;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            fps = accum / frames;

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        if (fps < 30)
            style.normal.textColor = Color.yellow;
        else
            if (fps < 10)
                style.normal.textColor = Color.red;
            else
                style.normal.textColor = Color.green;
        string text = string.Format("{0:F2} FPS", fps);
        GUI.Label(rect, text, style);

        Rect memoryRect = new Rect(0, h * 2 / 50, w, h * 2 / 50);
        string memoryText = string.Format("Mono : {0:F2} MB", Profiler.GetMonoUsedSize() / (1024 * 1024f));
        GUI.Label(memoryRect, memoryText, style);

        //		string format  = "";
        //		if(WorldTextureProvider.IsCanUseProvider)
        //		{
        //			Texture2D tex = WorldTextureProvider.Instance.GetAtlasTexture(WorldTextureType.OpaqueTileTex);
        //			format = tex.format.ToString();
        //		}


        GameObject myPlayer = (HasActionObjectManager.Instance.getManager(HasActionObjectManagerTypes.PLAYER) as PlayerManager).getMyPlayer();
        WorldPos playerPos = myPlayer.GetComponent<GOPlayerController>().gameObjectState.CurPos;
        int biomeId = world.GetBiomeId(playerPos.x, playerPos.z);
        string biomeName = WorldConfig.Instance.GetBiomeConfigById(biomeId).name;
        string playerStateText = string.Format("位置:{0},群落ID:{1},群落Name:{2},aoId:{3},time:{4}", playerPos.ToString(), biomeId, biomeName, myPlayer.GetComponent<BaseAttributes>().aoId, DayNightTime.Instance.TotalTime);
        Rect playerStateRect = new Rect(0, h * 4 / 50, w, h * 2 / 50);
        GUI.Label(playerStateRect, playerStateText, style);
        if (GUI.Button(new Rect(w - 200, 0, 100, 50), "保存地图数据并退出"))
        {
            World.world.WorlderLoader.SaveAll();
        }

        //////////////////////////////////////////////////

        string editorModeStr = isEditorMode ? "退出编辑模式" : "进入编辑模式";
        if (GUI.Button(new Rect(w - 200, 50, 100, 50), editorModeStr))
        {
            if (!isEditorMode)
            {
                MTBEditorModeController.Instance.initFileLoader();
                if (editorUI == null)
                    editorUI = GameObject.Instantiate(Resources.Load("UI/EditorUI/EditorMainUI")) as GameObject;
                editorUI.SetActive(true);
                MTBEditorModeController.Instance.initController();
                isEditorMode = true;
            }
            else
            {
                MTBEditorModeController.Instance.dispose();
                Destroy(editorUI);
                isEditorMode = false;
            }
        }

        //        if(GUI.Button(new Rect(w - 200,0,100,50),"刷怪"))
        //        {
        //            float offset = 1;
        //            Vector3 pos = new Vector3(Random.Range(playerPos.x - offset,playerPos.x + offset),playerPos.y,Random.Range(playerPos.z - offset,playerPos.z + offset));
        //            (HasActionObjectManager.Instance.getManager(HasActionObjectManagerTypes.MONSTER) as MonsterManager).InitMonster(pos,10);
        //        }
        //        if (GUI.Button(new Rect(w - 200, 0, 100, 50), "相机测试"))
        //        {
        //            PlotCameraController.Instance.runScript(1, CameraManager.Instance.CurCamera);
        //        }
        //		if(GUI.Button(new Rect(w - 300,0,100,50),"清理内存"))
        //		{
        //			System.GC.Collect();
        //			Resources.UnloadUnusedAssets();
        //		}
        //		if(GUI.Button(new Rect(w - 400,0,100,50),NetManager.Instance.isServer ? "关闭服务" : "开启服务器"))
        //		{
        //			if(!NetManager.Instance.isServer)
        //			{
        //				NetManager.Instance.StartServer();
        //			}
        //			else
        //			{
        //				NetManager.Instance.StopServer();
        //			}
        //		}

        //		if(GUI.Button(new Rect(w - 500,0,100,50),"发送数据包"))
        //		{
        //			NetManager.Instance.client.SendPackage(PackageFactory.GetPackage(PackageType.LinkState));
        //		}

        //		if(GUI.Button(new Rect(w - 600,0,100,50),"刷新玩家"))
        //		{
        //			PlayerInfo info = new PlayerInfo();
        //			info.aoId = HasActionObjectManager.Instance.playerManager.CreateAoId();
        //			info.isMe = false;
        //			HasActionObjectManager.Instance.playerManager.InitPlayer(info);
        //		}

        //		bool isFreeMode = MainPlayerController.Instance.CurAttachController.isFreeMode;
        //		string freeModeStr = isFreeMode ? "退出飞行模式" : "进入飞行模式";
        //		if(GUI.Button(new Rect(w - 700,0,100,50),freeModeStr))
        //		{
        //			MainPlayerController.Instance.CurAttachController.ChangeToFree(!isFreeMode);
        //		}

        //		if(GUI.Button(new Rect(200,0,100,50),"停止线程"))
        //		{
        //			World.world.WorldGenerator.Dispose();
        //			Debug.Log("stop!");
        //		}

    }

}

