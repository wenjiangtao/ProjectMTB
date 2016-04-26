using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using MTB;

public class Game : MonoBehaviour
{
    public string[] wordconfig = new string[] { "WorldConfig", "WorldConfig0", "WorldConfig99", "本地测试" };
    public static Game instance;
    private PlayerInfo playerInfo;
    private NetType netType;

    void Awake()
    {
        instance = this;
        EventManager.RegisterEvent(EventMacro.SELECT_WORLD, onSelectWorld);
    }

    private void onSelectWorld(params object[] paras)
    {
        string worldConfigStr = (string)paras[1];
        string worldName = (string)paras[2];
        int seed = (int)paras[3];
        ActionDataManager.Instance.Init();
        InputActionDataManager.Instance.Init();
        DataManagerM.Instance.Init();
        ModelDataManager.Instance.Init();

        GameObject worldConfig = GameObject.Instantiate(Resources.Load("Prefabs/" + worldConfigStr) as GameObject);
        WorldConfig.Instance.seed = seed;
        WorldConfig.Instance.InitSavedPath(worldConfigStr, worldName);
        GameObject.Instantiate(Resources.Load("Prefabs/SkyBox/DayNightTime") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/SkyBox/SkyBox") as GameObject);
		if(WorldConfig.Instance.savedPath.Contains("剧情模式"))
		{
			GameObject.Instantiate(Resources.Load("Prefabs/FlyShipParts") as GameObject);
		}
        WallOfAirManager.Instance.Init();
        netType = (NetType)paras[0];

        if (netType == NetType.Local)
        {
            playerInfo = (PlayerInfo)paras[4];
            DayNightTime.Instance.SetNetTime(true);
            float worldTime = (float)paras[5];
            DayNightTime.Instance.UpdateTime(worldTime);
        }
        else
        {
            playerInfo = new PlayerInfo();
            playerInfo.isMe = true;
            playerInfo.aoId = AoIdManager.instance.getAoId();
            playerInfo.position = WorldConfig.Instance.birthPlace;
            DayNightTime.Instance.SetNetTime(false);
        }
        WorldTextureProvider.Instance.Pack();
        EventManager.RegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, OnFirstWorldGeneratedFinish);
        World.Generate("Prefabs/World", playerInfo.position, netType);
    }

    private void OnFirstWorldGeneratedFinish(params object[] paras)
    {
        EventManager.UnRegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, OnFirstWorldGeneratedFinish);
        MineController.Instance.Init();
        BlockExplodeController.Instance.Init();
        DropController.Instance.Init();
        BlockMaskController.Instance.Init();
        HasActionObjectManager.Instance.playerManager.InitPlayer(playerInfo);
        UIManager.Instance.showUI<MainUI>(UITypes.MAIN_UI);
        GameObject.Instantiate(Resources.Load("Prefabs/CameraContainer") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/MainPlayerController") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/HUDFPS") as GameObject);

        //进入游戏之后，将UI相机清楚模式改下，因为它会与游戏中相机冲突
        GameUIRoot.RootGo.transform.Find("UICamera").GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
        WeatherController.Instance.Init();
        MTBTaskController.Instance.Init();

        EventManager.SendEvent(EventMacro.ON_JOIN_SCENE_SUCCESS);
    }
}


