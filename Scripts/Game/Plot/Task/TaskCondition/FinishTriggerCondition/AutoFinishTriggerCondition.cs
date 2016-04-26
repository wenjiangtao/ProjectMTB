using System;
using System.Collections.Generic;
namespace MTB
{
    public class AutoFinishTriggerCondition : BaseTaskCondition
    {
        public override void setParams(Dictionary<string, string> paras)
        {
            InitEvent();
        }

        private void InitEvent()
        {
            EventManager.RegisterEvent(EventMacro.TASK_AUTO_START, onAutoStart);
        }

        private void removeEvent()
        {
            EventManager.UnRegisterEvent(EventMacro.TASK_AUTO_START, onAutoStart);
        }


        public override bool MeetCondition()
        {
            return MTBTaskController.Instance.meetFinishCondition();
        }

        private void onAutoStart(params object[] paras)
        {
            int taskid = Convert.ToInt32(paras[0]);
            int stepid = Convert.ToInt32(paras[1]);
            if (taskid == taskId && stepid == stepId && MeetCondition())
            {
                removeEvent();
                MTBArrowManager.Instance.disposeArrow();
                MTBTaskController.Instance.finishStep(taskId, stepId);
            }
        }
    }
}
