using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class AutoStartCondition : BaseTaskCondition
    {
        public override void setParams(Dictionary<string, string> paras)
        {
            isMeet = false;
            base.setParams(paras);
        }


        public override bool MeetCondition()
        {
            MTBArrowManager.Instance.initTaskArrow(taskId, stepId);
            HasActionObjectManager.Instance.playerManager.getMyPlayer().GetComponent<GOPlayerController>().StopMove();
            MTBTaskController.Instance.startTask(taskId, stepId);
            EventManager.SendEvent(EventMacro.TASK_AUTO_START, taskId, stepId);
            isMeet = true;
            return true;
        }
    }
}
