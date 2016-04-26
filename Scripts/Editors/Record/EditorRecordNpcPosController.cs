using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTB;

public class EditorRecordNpcPosController : MonoBehaviour
{
    private string doc;
    private string npcID;
    private string removeDataId;
    private bool enableMark;
    private int index;
    private NPCPosData tempData;

    void Awake()
    {
        doc = "";
        npcID = "1";
        removeDataId = "1";
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            GUI.Label(new Rect(w - 100, h / 2 - 120, 100, 20), "id:" + index);
            GUI.Label(new Rect(w - 100, h / 2 - 100, 100, 20), "NPC注释");
            doc = GUI.TextField(new Rect(w - 100, h / 2 - 80, 200, 20), doc, 10);
            GUI.Label(new Rect(w - 100, h / 2 - 60, 100, 20), "NPCid");
            npcID = GUI.TextField(new Rect(w - 100, h / 2 - 40, 200, 20), npcID, 10);
            if (GUI.Button(new Rect(w - 100, h / 2 - 20, 100, 20), "放置npc"))
            {
                if (npcID != null && npcID != "")
                {
                    npcID = Regex.Replace(npcID, "[a-zA-Z]", "");
                    tempsave(Convert.ToInt32(npcID));
                }
            }

            if (GUI.Button(new Rect(w - 100, h / 2, 100, 20), "保存NPC数据"))
            {
                save();
            }

            GUI.Label(new Rect(w - 100, h / 2 + 20, 100, 20), "id");
            removeDataId = GUI.TextField(new Rect(w - 100, h / 2 + 40, 100, 20), removeDataId, 10);
            if (GUI.Button(new Rect(w - 100, h / 2 + 60, 100, 20), "删除npc位置数据"))
            {
                removeDataId = Regex.Replace(removeDataId, "[a-zA-Z]", "");
                NPCPosDataManager.Instance.removeData(Convert.ToInt32(removeDataId));
            }

            if (GUI.Button(new Rect(w - 100, h / 2 + 80, 100, 20), "清除场景中所有npc"))
            {
                HasActionObjectManager.Instance.npcManager.RemoveObjBeside(-1);
            }

            //if (GUI.Button(new Rect(w - 100, h / 2 + 20, 100, 20), "清除所有npc"))
            //{
            //    HasActionObjectManager.Instance.npcManager.RemoveObjBeside(-1);
            //}
        }
    }

    private void tempsave(int npcid)
    {
        index = NPCPosDataManager.Instance.getInsertId();
        NPCPosData data = new NPCPosData();
        data.doc = doc;
        data.id = index;
        data.npcId = npcid;
        data.pos = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
        tempData = data;
        NPCInfo info = new NPCInfo();
        info.aoId = AoIdManager.instance.getAoId();
        info.NPCId = data.npcId;
        info.position = data.pos;
        HasActionObjectManager.Instance.npcManager.InitNPC(info, false);
    }

    private void save()
    {
        if (tempData == null) return;
        NPCPosDataManager.Instance.addSaveData(tempData);
        index = NPCPosDataManager.Instance.getInsertId();
        tempData = null;
    }

    public void startSetNpcPos()
    {
        enableMark = true;
        NPCPosDataManager.Instance.openDocument();
        index = NPCPosDataManager.Instance.getInsertId();
    }

    public void cancel()
    {
        enableMark = false;
        NPCPosDataManager.Instance.saveData();
    }
}

