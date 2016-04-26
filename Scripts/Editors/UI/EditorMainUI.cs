using UnityEngine;
using MTB;
public class EditorMainUI : MonoBehaviour
{
    void Start()
    {
        //gameObject.AddComponent<EditorMainBag>();
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        bool recordMode = MTBEditorModeController.Instance.curMode() == MTBEditorModeType.RECORD;
        string redordModeStr = recordMode ? "退出记录" : "开始记录";
        if (GUI.Button(new Rect(0, h - 50, 150, 30), redordModeStr))
        {
            if (recordMode)
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.EDITOR);
            else
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.RECORD);
        }
        if (!recordMode)
        {
            if (GUI.Button(new Rect(0, h - 90, 100, 30), "自由编辑(E)"))
            {
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.EDITOR);
            }
            if (GUI.Button(new Rect(0, h - 130, 100, 30), "选择区域(Q)"))
            {
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.SELECT);
            }
            if (GUI.Button(new Rect(0, h - 170, 100, 30), "填充(Z)"))
            {
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.FILL);
            }
            if (GUI.Button(new Rect(0, h - 210, 100, 30), "拖拽选区(F)"))
            {
                EditorSelectArea.Instance.cutMark = true;
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.DRAG);
            }
            if (GUI.Button(new Rect(0, h - 250, 100, 30), "复制选区(V)"))
            {
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.changeMode(MTBEditorModeType.DRAG);
            }
            if (GUI.Button(new Rect(0, h - 290, 100, 30), "保存选区到文件"))
            {
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.saveData();
            }
            if (GUI.Button(new Rect(0, h - 330, 100, 30), "载入预制文件"))
            {
                if (EditorSelectArea.Instance.hasData())
                    EditorSelectArea.Instance.updateLastSelectView();
                EditorSelectArea.Instance.cutMark = false;
                MTBEditorModeController.Instance.loadData();
            }

            if (GUI.Button(new Rect(0, h - 365, 100, 30), "设置为出生点"))
            {
                MTBEditorModeController.Instance.saveBirthPos();
            }
            if (GUI.Button(new Rect(0, h - 400, 100, 30), "保存场景"))
            {
                World.world.saveAll();
            }
        }
        bool isFreeMode = MainPlayerController.Instance.CurAttachController.isFreeMode;
        string freeModeStr = isFreeMode ? "退出飞行模式" : "进入飞行模式";
        if (GUI.Button(new Rect(w / 2 - 50, 0, 100, 50), freeModeStr))
        {
            MainPlayerController.Instance.CurAttachController.ChangeToFree(!isFreeMode);
        }

        //if (GUI.Button(new Rect(w / 2 + 50, 0, 100, 50), "npc"))
        //{
        //    NPCInfo info = new NPCInfo();
        //    info.position = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
        //    info.NPCId = 1;
        //    info.aoId = AoIdManager.instance.getAoId();
        //    HasActionObjectManager.Instance.npcManager.InitNPC(info);
        //}

        if (Input.GetKeyUp(KeyCode.Q))
        {
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.SELECT);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.EDITOR);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            EditorSelectArea.Instance.cutMark = true;
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.DRAG);
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            EditorSelectArea.Instance.cutMark = false;
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.DRAG);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.FILL);
        }
    }

    public void dispose()
    {
        MTBEditorModeController.Instance.cancel();
    }
}
