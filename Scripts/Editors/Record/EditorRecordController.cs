using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTB;

public class EditorRecordController : MonoBehaviour, IEditorController
{
    private bool enableMark;
    private int state;
    private EditorRecordNpcPosController npcPosRecord;
    private EditorRecordPathController cameraRecord;
    private EditorRecordSpecialPosController posRecord;
    private EditorRecordArrowPosController arrowRecord;
    private string taskStr;
    private string stepStr;

    public EditorRecordController()
    {
        npcPosRecord = gameObject.AddComponent<EditorRecordNpcPosController>();
        cameraRecord = gameObject.AddComponent<EditorRecordPathController>();
        posRecord = gameObject.AddComponent<EditorRecordSpecialPosController>();
        arrowRecord = gameObject.AddComponent<EditorRecordArrowPosController>();
        taskStr = "1";
        stepStr = "1";
    }

    public MTBEditorModeType editorType()
    {
        return MTBEditorModeType.RECORD;
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            if (state == 1)
                checkStart(w, h);
            if (state == 2)
                checkNPC(w, h);
            if (state == 3)
                checkCamera(w, h);
            if (state == 4)
                checkArrow(w, h);
        }
    }

    private void checkStart(int w, int h)
    {
        if (GUI.Button(new Rect(w - 150, h / 2 - 100, 150, 40), "开始记录NPC坐标"))
        {
            npcPosRecord.startSetNpcPos();
            state = 2;
        }

        if (GUI.Button(new Rect(w - 150, h / 2 - 50, 150, 40), "开始记录相机路径"))
        {
            cameraRecord.startCameraRecord();
            state = 3;
        }

        if (GUI.Button(new Rect(w - 150, h / 2 - 150, 150, 40), "开始记录箭头坐标"))
        {
            arrowRecord.startSetArrowPos();
            state = 4;
        }

        GUI.Label(new Rect(w - 150, h / 2, 150, 20), "taskId");
        taskStr = GUI.TextField(new Rect(w - 150, h / 2 + 20, 150, 20), taskStr);
        if (GUI.Button(new Rect(w - 150, h / 2 + 40, 150, 20), "重置任务"))
        {
            if (taskStr != null && taskStr != "" && stepStr != null && stepStr != "")
            {
                taskStr = Regex.Replace(taskStr, "[a-zA-Z]", "");
                MTBTaskController.Instance.resetTask(Convert.ToInt32(taskStr));
            }
        }
    }

    private void checkNPC(int w, int h)
    {
        if (GUI.Button(new Rect(w - 200, h / 2 + 80, 200, 40), "结束并保存临时保存的数据"))
        {
            npcPosRecord.cancel();
            state = 1;
        }
    }


    private void checkCamera(int w, int h)
    {
        if (GUI.Button(new Rect(w - 200, h / 2 + 80, 200, 40), "结束并保存临时保存的数据"))
        {
            cameraRecord.cancel();
            state = 1;
        }
    }

    private void checkArrow(int w, int h)
    {
        if (GUI.Button(new Rect(w - 200, h / 2 + 80, 200, 40), "结束并保存临时保存的数据"))
        {
            arrowRecord.cancel();
            state = 1;
        }
    }

    public void setState(int state) { }
    public void cancel()
    {
        enableMark = false;
        npcPosRecord.cancel();
        cameraRecord.cancel();
        arrowRecord.cancel();
    }
    public void enable()
    {
        state = 1;
        enableMark = true;
    }
    public void disEnable()
    {
        cancel();
    }
}

