/*****
 * 
 * 管理ARROW
 * 
 * ****/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace MTB
{
    public class MTBArrowManager : Singleton<MTBArrowManager>
    {
        private static string TIPAREAPREFABPATH = "Effects/Game_Effects/E_TaskGlow";
        private static string ARROWPREFABPATH = "Effects/Game_Effects/E_xs_jiantou_1";
        private Dictionary<Vector3, GameObject> sceneArrowMap;
        public GameObject tipsLightObj;

        void Awake()
        {
            gameObject.name = "ArrowManager";
            sceneArrowMap = new Dictionary<Vector3, GameObject>();
        }

        public void initTaskArrow(int taskId, int stepId)
        {
            bool mark = MTBTaskController.Instance.conditionMeetHelper.curStepCondition().tipStr.Split('-').Length > 1;
            if (mark)
            {
                tipsLightObj = GameObject.Instantiate(Resources.Load(TIPAREAPREFABPATH) as GameObject);
                tipsLightObj.transform.position = MTBTaskDataManager.Instance.getData(taskId).stepList[stepId].npcPosData.pos + new Vector3(0, 0.5f, 0);
                tipsLightObj.transform.parent = gameObject.transform;
            }
            string arrowId = taskId + "-" + stepId;
            MTBArrowData data = MTBArrowDataManager.Instance.getData(arrowId);
            if (data == null)
                return;
            foreach (StepArrowData stepdata in data.ArrowList)
            {
                GameObject arrow = GameObject.Instantiate(Resources.Load(ARROWPREFABPATH) as GameObject);
                arrow.transform.position = new Vector3(stepdata.position.x, stepdata.position.y + 0.5f, stepdata.position.z);
                arrow.transform.eulerAngles = new Vector3(arrow.transform.eulerAngles.x, stepdata.rotation, arrow.transform.eulerAngles.z);
                arrow.transform.parent = gameObject.transform;
                sceneArrowMap.Add(arrow.transform.position, arrow);
            }
        }

        public void disposeArrow()
        {
            List<Vector3> keys = new List<Vector3>(sceneArrowMap.Keys);
            foreach (Vector3 key in keys)
            {
                sceneArrowMap[key].SetActive(false);
                SelfDestroy.Destroy(sceneArrowMap[key]);
            }
            sceneArrowMap.Clear();
            if (tipsLightObj != null)
                SelfDestroy.Destroy(tipsLightObj);
        }
    }
}
