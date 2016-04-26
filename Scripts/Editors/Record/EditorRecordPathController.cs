using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTB;

public class EditorRecordPathController : MonoBehaviour
{
    private List<CameraMoveStep> pathList;
    private CameraMoveData curData;
    private CameraStartPos startPos;
    private string time;
    private string name;
    private int stepIndex;
    private bool enableMark;
    private int state;
    private int index;
    private string removeIndex;

    void Awake()
    {
        pathList = new List<CameraMoveStep>();
        enableMark = false;
        name = "";
        time = "1";
        removeIndex = "1";
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            if (state == 1)
            {
                if (GUI.Button(new Rect(w - 100, h / 2 + 20, 100, 20), "开始记录路径"))
                {
                    startRecord();
                    state = 2;
                }
                GUI.Label(new Rect(w - 100, h / 2 - 80, 100, 20), "id");
                removeIndex = GUI.TextField(new Rect(w - 100, h / 2 - 60, 100, 20), removeIndex);
                if (GUI.Button(new Rect(w - 100, h / 2 - 40, 100, 20), "删除路径"))
                {
                    if (removeIndex != null && removeIndex != "")
                    {
                        removeIndex = Regex.Replace(removeIndex, "[a-zA-Z]", "");
                        CameraMoveDataManager.Instance.removeData(Convert.ToInt32(removeIndex));
                    }
                    index = CameraMoveDataManager.Instance.getInsertId();
                }
                if (GUI.Button(new Rect(w - 100, h / 2 - 20, 100, 20), "尝试播放路径"))
                {
                    if (removeIndex != null && removeIndex != "")
                    {
                        removeIndex = Regex.Replace(removeIndex, "[a-zA-Z]", "");
                        CameraMoveData data = CameraMoveDataManager.Instance.getData(Convert.ToInt32(removeIndex));
                        PlotCameraController.Instance.runScript(data, true);
                    }
                }

            }
            if (state == 2)
            {
                GUI.Label(new Rect(w - 100, h / 2 - 120, 100, 20), "id:" + index);
                GUI.Label(new Rect(w - 100, h / 2 - 100, 100, 20), "路径注释");
                name = GUI.TextField(new Rect(w - 100, h / 2 - 80, 200, 20), name, 10);
                if (GUI.Button(new Rect(w - 100, h / 2 - 60, 100, 20), "完成"))
                {
                    tempsavePath(name);
                    state = 3;
                }
                GUI.Label(new Rect(w - 100, h / 2 - 20, 100, 20), "移动时间");
                time = GUI.TextField(new Rect(w - 100, h / 2, 200, 20), time, 10);
                if (GUI.Button(new Rect(w - 100, h / 2 + 20, 100, 20), "加入到路径"))
                {
                    if (time != null && time != "")
                    {
                        time = Regex.Replace(time, "[a-zA-Z]", "");
                        recordNextPosition((float)Convert.ToInt32(time));
                    }
                }
            }
            if (state == 3)
            {
                if (GUI.Button(new Rect(w - 100, h / 2 + 50, 100, 20), "回放路径"))
                {
                    EventManager.RegisterEvent(PlotEvent.CAMERAMOVEFINISH, onCameraMoveFinish);
                    PlotCameraController.Instance.runScript(curData, true);
                }

                if (GUI.Button(new Rect(w - 100, h / 2 + 10, 100, 20), "临时保存"))
                {
                    savePath();
                    state = 1;
                }

                if (GUI.Button(new Rect(w - 100, h / 2 - 20, 100, 20), "取消路径"))
                {
                    state = 1;
                    curData = null;
                }
            }
            if (state == 4)
            {
                if (GUI.Button(new Rect(w - 100, h / 2 + 10, 100, 20), "临时保存"))
                {
                    savePath();
                    state = 1;
                }

                if (GUI.Button(new Rect(w - 100, h / 2 - 20, 100, 20), "取消路径"))
                {
                    state = 1;
                    curData = null;
                }
            }
        }
    }

    private void onCameraMoveFinish(params object[] paras)
    {
        state = 4;
    }

    public void cancel()
    {
        curData = null;
        enableMark = false;
        state = 1;
        CameraMoveDataManager.Instance.saveData();
        EventManager.UnRegisterEvent(PlotEvent.CAMERAMOVEFINISH, onCameraMoveFinish);
    }

    public void startCameraRecord()
    {
        pathList.Clear();
        enableMark = true;
        index = CameraMoveDataManager.Instance.getInsertId();
        CameraMoveDataManager.Instance.openDocument();
    }

    public void startRecord()
    {
        index = CameraMoveDataManager.Instance.getInsertId();
        pathList.Clear();
        stepIndex = 1;
        startPos = new CameraStartPos();
        startPos.position = CameraManager.Instance.CurCamera.transform.position;
        Vector3 temp = CameraManager.Instance.CurCamera.transform.eulerAngles;
        startPos.rotation = CameraManager.Instance.CurCamera.transform.eulerAngles;
    }

    private void recordNextPosition(float time)
    {
        CameraMoveStep step = new CameraMoveStep();
        step.id = stepIndex;
        step.position = CameraManager.Instance.CurCamera.transform.position;
        step.rotation = CameraManager.Instance.CurCamera.transform.eulerAngles;
        step.time = time;
        pathList.Add(step);
        stepIndex++;
    }

    private void tempsavePath(string name)
    {
        if (curData == null)
            curData = new CameraMoveData();
        curData.name = name;
        curData.id = CameraMoveDataManager.Instance.getInsertId();
        curData.startpos = startPos;
        curData.steps = pathList;
    }

    private void savePath()
    {
        CameraMoveData data = new CameraMoveData();
        data.name = curData.name;
        data.id = CameraMoveDataManager.Instance.getInsertId();
        data.startpos = startPos;
        data.steps = new List<CameraMoveStep>();
        for (int i = 0; i < pathList.Count; i++)
        {
            data.steps.Add(pathList[i]);
        }
        CameraMoveDataManager.Instance.addSaveData(data);
        curData = null;
        index = CameraMoveDataManager.Instance.getInsertId();
    }
}

