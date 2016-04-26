using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTB;

public class EditorRecordArrowPosController : MonoBehaviour
{
    private string name;
    private string arrowID;
    private string removeDataId;
    private bool enableMark;
    private int index;
    private string taskId;
    private string stepId;
    private List<StepArrowData> listArrow;

    void Awake()
    {
        listArrow = new List<StepArrowData>();
        name = "";
        arrowID = "1-1";
        removeDataId = "1-1";
        taskId = "1";
        stepId = "1";
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            GUI.Label(new Rect(w - 100, h / 2 - 120, 100, 20), "任务id:");
            taskId = GUI.TextField(new Rect(w - 100, h / 2 - 100, 200, 20), taskId, 10);
            GUI.Label(new Rect(w - 100, h / 2 - 80, 100, 20), "步骤id:");
            stepId = GUI.TextField(new Rect(w - 100, h / 2 - 60, 200, 20), stepId, 10);

            if (GUI.Button(new Rect(w - 100, h / 2 - 40, 100, 20), "放置箭头"))
            {
                if (taskId != null && taskId != "" && stepId != null && stepId != "")
                {
                    taskId = Regex.Replace(taskId, "[a-zA-Z]", "");
                    stepId = Regex.Replace(stepId, "[a-zA-Z]", "");
                    tempsave(Convert.ToInt32(taskId), Convert.ToInt32(stepId));
                }
            }
            if (GUI.Button(new Rect(w - 100, h / 2 - 20, 100, 20), "保存箭头数据"))
            {
                save();
            }
            GUI.Label(new Rect(w - 100, h / 2, 100, 20), "id");
            if (GUI.Button(new Rect(w - 100, h / 2 + 20, 100, 20), "删除箭头数据"))
            {
                taskId = Regex.Replace(taskId, "[a-zA-Z]", "");
                stepId = Regex.Replace(stepId, "[a-zA-Z]", "");
                removeDataId = taskId + "-" + stepId;
                MTBArrowDataManager.Instance.removeData(removeDataId);
            }
            if (GUI.Button(new Rect(w - 100, h / 2 + 40, 100, 20), "清除场景中所有箭头"))
            {
                //HasActionObjectManager.Instance.npcManager.RemoveObjBeside(-1);
            }
        }
    }

    private void tempsave(int taskid, int stepid)
    {
        string idStr = taskid + "-" + stepid;
        if (!MTBArrowDataManager.Instance.checkIdCanUse(idStr))
        {
            listArrow.Clear();
            Debug.LogError("当前任务步骤 " + taskid + "," + stepid + " 箭头已经配置，请删除对应箭头配置后再添加");
            return;
        }
        StepArrowData arrowData = new StepArrowData();
        arrowData.position = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
        arrowData.rotation = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.eulerAngles.y;
        listArrow.Add(arrowData);
    }

    private void save()
    {
        MTBArrowData data = new MTBArrowData();
        for (int i = 0; i < listArrow.Count; i++)
        {
            data.ArrowList.Add(listArrow[i]);
        }
        data.id = taskId + "-" + stepId;
        MTBArrowDataManager.Instance.addSaveData(data);
        listArrow.Clear();
    }

    public void startSetArrowPos()
    {
        enableMark = true;
        listArrow.Clear();
        MTBArrowDataManager.Instance.openDocument();
    }

    public void cancel()
    {
        enableMark = false;
        MTBArrowDataManager.Instance.saveData();
    }
}
