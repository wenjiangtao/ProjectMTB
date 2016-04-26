using UnityEngine;
using System.Collections;
using MTB;

public class MTBSceneEditors : MonoBehaviour
{
    public string worldConfigStr;
    //使用编辑器记录的出生位置
    public bool useSaveBirthPlace = false;

    private PlayerInfo playerInfo;
    void Start()
    {
        gameObject.AddComponent<Watcher>();
        GameObject.Instantiate(Resources.Load("Prefabs/GameConfig") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject);

        ActionDataManager.Instance.Init();
        InputActionDataManager.Instance.Init();
        DataManagerM.Instance.Init();
        ModelDataManager.Instance.Init();

        GameObject worldConfig = GameObject.Instantiate(Resources.Load("Prefabs/" + worldConfigStr) as GameObject);
        WorldConfig.Instance.InitSavedPath(worldConfigStr);

        GameObject.Instantiate(Resources.Load("Prefabs/SkyBox/DayNightTime") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/SkyBox/SkyBox") as GameObject);

        MTBEditorModeController.Instance.initFileLoader();
        playerInfo = new PlayerInfo();
        playerInfo.isMe = true;
		playerInfo.aoId = AoIdManager.instance.getAoId();

        Vector3 playerPos = MTBEditorModeController.Instance.loadBirthPos(useSaveBirthPlace);
        playerInfo.position = playerPos;

        WorldTextureProvider.Instance.Pack();
        EventManager.RegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, OnFirstWorldGeneratedFinish);
        World.Generate("Prefabs/WorldEditor", playerPos, NetType.Single);

        HasActionObjectManager.Instance.playerManager.InitPlayer(playerInfo);
        GameObject.Instantiate(Resources.Load("Prefabs/CameraContainer") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/MainPlayerController") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/MTBUserInput") as GameObject);
        GameObject.Instantiate(Resources.Load("UI/EditorUI/EditorMainUI"));

        MTBEditorModeController.Instance.initController();
        MTBUserInput.Instance.JoyStickView.SetActive(false);
    }

    private void OnFirstWorldGeneratedFinish(params object[] paras)
    {
        EventManager.UnRegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, OnFirstWorldGeneratedFinish);
    }
}
