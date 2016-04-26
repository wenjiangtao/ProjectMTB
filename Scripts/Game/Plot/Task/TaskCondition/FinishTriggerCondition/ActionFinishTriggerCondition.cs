using System;
using System.Collections.Generic;
namespace MTB
{
    public class ActionFinishTriggerCondition : BaseTaskCondition
    {
        private string actionName;

        public override void setParams(Dictionary<string, string> paras)
        {
            paras.TryGetValue("params", out actionName);
            InitEvent();
        }

        public override bool MeetCondition()
        {
            return MTBTaskController.Instance.meetFinishCondition();
        }

        private void InitEvent()
        {
            EventManager.RegisterEvent(PlotEvent.ACTIONFINISH, onActionFinish);
        }

        private void removeEvent()
        {
            EventManager.UnRegisterEvent(PlotEvent.ACTIONFINISH, onActionFinish);
        }

        private void onActionFinish(params object[] paras)
        {
            int aoId = Convert.ToInt32(paras[0]);
            int taskid = Convert.ToInt32(paras[1]);
            int stepid = Convert.ToInt32(paras[2]);
            string actionN = Convert.ToString(paras[3]);
            if (actionN.Equals(actionName))
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
