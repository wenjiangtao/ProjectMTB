using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class DistanceStartCondition : BaseTaskCondition
    {
        private GameObject npc;
        private int npcAoId;
        private float targetDistance;
        private float curDistance;
        private IEnumerator coroutine;
        private bool meetMark;
        private float timeDelay;

        public override void setParams(Dictionary<string, string> paras)
        {
            string targetD;
            paras.TryGetValue("params", out targetD);
            npc = HasActionObjectManager.Instance.npcManager.getObjByTaskId(taskId, stepId);
            targetDistance = Convert.ToSingle(targetD);
            isMeet = false;
            meetMark = false;
            coroutine = checkCondition();
            base.setParams(paras);
            timeDelay = 0;
        }

        public override bool MeetCondition()
        {
            MTBArrowManager.Instance.initTaskArrow(taskId, stepId);
            MTBTaskController.Instance.conditionMeetHelper.StartCoroutine(coroutine);
            isMeet = true;
            return true;
        }

        private void stopCondition()
        {
            MTBTaskController.Instance.conditionMeetHelper.StopCoroutine(coroutine);
        }

        private IEnumerator checkCondition()
        {
            while (true)
            {
                if (!meetMark)
                {
                    if (npc == null)
                        npc = HasActionObjectManager.Instance.npcManager.getObjByTaskId(taskId, stepId);
                    if (npc != null)
                    {
                        Vector3 pos = npc.transform.position;
                        Vector3 myPos = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
                        curDistance = Vector3.Distance(pos, myPos);
                        if (curDistance <= targetDistance)
                        {
                            meetMark = true;

                        }
                    }
                }
                else
                {
                    timeDelay += Time.deltaTime;
                    if (timeDelay >= 0.2f)
                    {
                        meetMark = false;
                        timeDelay = 0;
                        stopCondition();
                        HasActionObjectManager.Instance.playerManager.getMyPlayer().GetComponent<GOPlayerController>().StopMove();
                        MTBTaskController.Instance.startTask(taskId, stepId);
                        EventManager.SendEvent(EventMacro.TASK_AUTO_START, taskId, stepId);
                    }
                }
                yield return null;
            }
        }

        public override void dispose()
        {
            stopCondition();
            base.dispose();
        }
    }
}
