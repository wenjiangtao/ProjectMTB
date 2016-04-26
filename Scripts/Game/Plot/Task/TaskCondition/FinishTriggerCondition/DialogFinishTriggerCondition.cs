using System;
using System.Collections.Generic;
namespace MTB
{
    public class DialogFinishTriggerCondition : BaseTaskCondition
    {
        public override void setParams(Dictionary<string, string> paras)
        {
            InitEvent();
        }

        public override bool MeetCondition()
        {
            return MTBTaskController.Instance.meetFinishCondition();
        }

        private void InitEvent()
        {
            EventManager.RegisterEvent(PlotEvent.DIALOGEFINISH, onDialogueFinish);
        }

        private void removeEvent()
        {
            EventManager.UnRegisterEvent(PlotEvent.DIALOGEFINISH, onDialogueFinish);
        }

        private void onDialogueFinish(params object[] paras)
        {
            int taskid = Convert.ToInt32(paras[0]);
            int stepid = Convert.ToInt32(paras[1]);
            if (taskid == taskId && stepId == stepid && MeetCondition())
            {
                removeEvent();
                MTBArrowManager.Instance.disposeArrow();
                MTBTaskController.Instance.finishStep(taskId, stepId);
            }
        }

        public override void dispose()
        {
            removeEvent();
            base.dispose();
        }
    }
}
