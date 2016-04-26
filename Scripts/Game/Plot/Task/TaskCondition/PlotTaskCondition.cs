/****
 * 包括一个任务各个步骤的开启结束以及各种condition
 * ***/
using System.Collections.Generic;

namespace MTB
{
    public class PlotTaskCondition
    {
        public int taskId { get; private set; }
        public Dictionary<int, PlotStepCondition> stepConditionList { get; private set; }

        public PlotTaskCondition()
        {
            stepConditionList = new Dictionary<int, PlotStepCondition>();
        }

        public void updateCondition(MTBTaskData data)
        {
            taskId = data.id;
            stepConditionList.Clear();
            foreach (MTBTaskStepData stepdata in data.stepList.Values)
            {
                stepConditionList.Add(stepdata.id, new PlotStepCondition(data, stepdata));
            }
        }

        public PlotStepCondition getStepCondition(int step)
        {
            PlotStepCondition stepCondition;
            stepConditionList.TryGetValue(step, out stepCondition);
            return stepCondition;
        }

        public void disPose()
        {
            stepConditionList.Clear();
            stepConditionList = null;
        }

    }

    public class PlotStepCondition
    {
        public BaseTaskCondition startTriggerCondition { get; private set; }
        public BaseTaskCondition finishTriggerCondition { get; private set; }
        public BaseTaskCondition finishCondition { get; private set; }
        public string tipStr { get; private set; }

        public PlotStepCondition(MTBTaskData taskData, MTBTaskStepData stepData)
        {
            MTBTaskConditionData data = MTBTaskConditionManager.Instance.getData(stepData.condtion);
            startTriggerCondition = TaskConditionFactory.GetStartTriggerCondition(data.startTriggerCondition.scriptName);
            startTriggerCondition.taskId = taskData.id;
            startTriggerCondition.stepId = stepData.id;
            startTriggerCondition.setParams(data.startTriggerCondition.paras);

            finishTriggerCondition = TaskConditionFactory.GetFinishTriggerCondition(data.finishTriggerCondition.scriptName);
            finishTriggerCondition.taskId = taskData.id;
            finishTriggerCondition.stepId = stepData.id;
            finishTriggerCondition.setParams(data.finishTriggerCondition.paras);

            finishCondition = TaskConditionFactory.GetFinishCondition(data.finishCondition.scriptName);
            finishCondition.taskId = taskData.id;
            finishCondition.stepId = stepData.id;
            finishCondition.setParams(data.finishCondition.paras);

            tipStr = data.tipsCondition.content;
        }

        public void dispose()
        {
            startTriggerCondition.dispose();
            startTriggerCondition = null;
            finishTriggerCondition.dispose();
            finishTriggerCondition = null;
            finishCondition.dispose();
            finishCondition = null;
        }
    }
}
