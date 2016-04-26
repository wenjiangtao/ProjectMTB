using UnityEngine;
using System;
using System.Collections.Generic;

namespace MTB
{
    public class MTBTaskCondtionMeetHelper : MonoBehaviour
    {
        public PlotTaskCondition curCondition { get; private set; }

        public List<string> curTaskTips { get; private set; }

        public Dictionary<int, PlotTaskCondition> conditionMap { get; private set; }

        private int MeetStep;

        void Awake()
        {
            MeetStep = MTBTaskController.Instance.curStepId == 0 ? 1 : MTBTaskController.Instance.curStepId;
            conditionMap = new Dictionary<int, PlotTaskCondition>(new ConditionHelperComparer());
            curTaskTips = new List<string>();
            updateStartCondition();
        }

        //目前只有一个
        public PlotStepCondition curStepCondition()
        {
            foreach (int key in conditionMap.Keys)
            {
                if (conditionMap[key].stepConditionList.ContainsKey(MeetStep))
                {
                    return conditionMap[key].stepConditionList[MeetStep];
                }
            }
            return null;
        }

        public bool startTask(int taskId)
        {
            if (conditionMap.ContainsKey(taskId))
            {
                curCondition = conditionMap[taskId];
                return true;
            }
            return false;
        }

        public bool finishTask(int taskId)
        {
            if (curCondition == null || curCondition.taskId != taskId)
                return false;
            curCondition = null;
            MeetStep = 1;
            return true;
        }

        public bool finishStep(int stepId)
        {
            if (MeetStep != stepId)
                return false;
            MeetStep = MTBTaskController.Instance.curStepId + 1;
            updateStartCondition();
            return true;
        }

        public void updateCondition(Dictionary<int, MTBTaskData> datalist)
        {
            conditionMap.Clear();
            foreach (int key in datalist.Keys)
            {
                if (conditionMap.ContainsKey(key))
                    conditionMap[key].updateCondition(datalist[key]);
                else
                {
                    PlotTaskCondition condition = new PlotTaskCondition();
                    condition.updateCondition(datalist[key]);
                    conditionMap.Add(key, condition);
                }
            }
            updateStartCondition();
        }

        private void updateStartCondition()
        {
            curTaskTips.Clear();
            foreach (int key in conditionMap.Keys)
            {
                if (conditionMap[key].stepConditionList.ContainsKey(MeetStep) && !conditionMap[key].stepConditionList[MeetStep].startTriggerCondition.isMeet)
                {
                    conditionMap[key].stepConditionList[MeetStep].startTriggerCondition.MeetCondition();
                    if (conditionMap[key].stepConditionList[MeetStep].tipStr != null && conditionMap[key].stepConditionList[MeetStep].tipStr != "")
                        curTaskTips.Add(conditionMap[key].stepConditionList[MeetStep].tipStr.Split('-')[0]);
                }
            }
        }

        public bool meetFinishCondition()
        {
            if (curCondition == null)
            {
                Debug.Log("试图结束没有初始化condition的任务");
                return false;
            }
            return curCondition.stepConditionList[MTBTaskController.Instance.curStepId].finishCondition.MeetCondition();
        }

        public void dispose()
        {
            foreach (int key in conditionMap.Keys)
            {
                if (conditionMap[key].stepConditionList.ContainsKey(MeetStep))
                {
                    conditionMap[key].stepConditionList[MeetStep].dispose();
                    conditionMap[key].disPose();
                }
            }
            conditionMap.Clear();
            MeetStep = 1;
        }
    }

    public class ConditionHelperComparer : IEqualityComparer<int>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<int>.Equals(int a, int b)
        {
            return a == b;
        }

        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return (int)obj;
        }
        #endregion
    }
}
