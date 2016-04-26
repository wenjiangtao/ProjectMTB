using System;
using System.Collections.Generic;
namespace MTB
{
    public class CameraMoveFinishTriggerCondition : BaseTaskCondition
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
            EventManager.RegisterEvent(PlotEvent.CAMERAMOVEFINISH, onCameraMoveFinish);
        }

        private void removeEvent()
        {
            EventManager.UnRegisterEvent(PlotEvent.CAMERAMOVEFINISH, onCameraMoveFinish);
        }

        private void onCameraMoveFinish(params object[] paras)
        {
            int taskid = Convert.ToInt32(paras[0]);
            int stepid = Convert.ToInt32(paras[1]);

            if (taskid == taskId && stepid == stepId && MeetCondition())
            {
                removeEvent();
                MTBArrowManager.Instance.disposeArrow();
                MTBTaskController.Instance.finishStep(taskId,stepId);
            }
        }

        public override void dispose()
        {
            removeEvent();
            base.dispose();
        }
    }
}
