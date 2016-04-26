using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class MTBTaskStepController
    {
        private MTBTaskData _taskData;
        private MTBTaskStepData _taskStepData;
        private TaskPanelController _taskPanelController;

        private int _curTaskId;
        private int _curStepId;

        public MTBTaskStepController()
        {
            _taskPanelController = new TaskPanelController();
        }

        public int curTaskId
        {
            get { return _curTaskId; }
            set { _curTaskId = value; }
        }

        public int curStepId
        {
            get { return _curStepId; }
            set { _curStepId = value; }
        }

        public void startStep(int taskId, int stepId)
        {
            curTaskId = taskId;
            curStepId = stepId;
            if (_taskData == null || _taskData.id != taskId)
                _taskData = MTBTaskDataManager.Instance.getData(_curTaskId);
            loadConditions();
            _taskPanelController.setCurTask(_taskData, curStepId);
            decodeScript(curStepId);
        }

        /***
         * 用于npc继续任务
         * ***/
        public void doStep(int stepId)
        {
            if (_taskStepData.nextstep == "end" || stepId != Convert.ToInt32(_taskStepData.nextstep))
                throw new Exception("任务步骤出错,taskid:" + curTaskId + ",stepid:" + curStepId);
            curStepId = stepId;
            decodeScript(curStepId);
        }

        /***
         * 用于检测任务是否完成
         * ***/
        public void finishStep()
        {
            //npc自己监听控制清除
            EventManager.SendEvent(EventMacro.TASK_FINISH_STEP, curTaskId, curStepId);
            if (_taskStepData.nextstep == "end")
                return;
            //刷出下一步的npc
            HasActionObjectManager.Instance.npcManager.InitNPC(_curTaskId, Convert.ToInt32(_taskStepData.nextstep));
        }

        public void checkFinishTask()
        {
            if (_taskStepData.nextstep == "end")
            {
                MTBTaskController.Instance.finishTask();
            }
        }

        private void decodeScript(int stepId)
        {
            loadConditions();
            _taskStepData = _taskData.stepList[stepId];
            if (_taskStepData.dialogId != 0)
            {
                showDialogPanel(Convert.ToInt32(_taskStepData.dialogId));
            }
            if (_taskStepData.cameraMoveId != 0)
            {
                showCameraEffect(Convert.ToInt32(_taskStepData.cameraMoveId));
            }
        }

        private void showDialogPanel(int dialogId)
        {
            _taskPanelController.startTaskDialog(dialogId);
            GameObject npc = HasActionObjectManager.Instance.npcManager.getObjByTaskId(this._curTaskId, this._curStepId);
            if (npc != null)
            {
                npc.transform.LookAt(HasActionObjectManager.Instance.playerManager.getMyPlayer().transform);
            }
        }

        private void showCameraEffect(int scriptId)
        {
            PlotCameraController.Instance.runScript(scriptId, CameraManager.Instance.CurCamera, curTaskId, curStepId);
        }

        protected void loadConditions()
        {

        }

        public bool meetCondition()
        {
            return false;
        }
    }
}
