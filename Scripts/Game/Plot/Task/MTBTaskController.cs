using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class MTBTaskController : Singleton<MTBTaskController>
    {
        private static string TaskUIPath = "UI/Task";
        protected MTBTaskCacheData _taskCacheData;
        protected int _curTaskId;
        protected MTBTaskStepController _stepController;
        //condition检测的是从上个任务结束到这个任务结束   包括未接取和接取
        private MTBTaskCondtionMeetHelper _conditionMeetHelper;
        public MTBTaskCondtionMeetHelper conditionMeetHelper { get { return _conditionMeetHelper; } }

        public void Init()
        {
            _curTaskId = 0;
            _taskCacheData = new MTBTaskCacheData();
            _stepController = new MTBTaskStepController();
            if (_taskCacheData.curStep != 0 && _taskCacheData.curConDuctTaskData != null)
            {
                _curTaskId = _taskCacheData.curConDuctTaskData.id;
                _stepController.curStepId = _taskCacheData.curStep;
            }
            _conditionMeetHelper = gameObject.AddComponent<MTBTaskCondtionMeetHelper>();
            _conditionMeetHelper.updateCondition(_taskCacheData.canConductTaskList);
            checkTaskUI();
        }

        public bool meetFinishCondition()
        {
            return _conditionMeetHelper.meetFinishCondition();
        }

        public MTBTaskCacheData taskCacherData
        {
            get { return _taskCacheData; }
            set { _taskCacheData = value; }
        }

        public bool isInTask
        {
            get { return _curTaskId != 0; }
        }

        public int curTaskId
        {
            get { return _curTaskId; }
        }

        public int curStepId
        {
            get { return _stepController.curStepId; }
        }

        public void startTask(int taskId)
        {
            startTask(taskId, 1);
        }

        public void startTask(int taskId, int stepId)
        {
            if (!_taskCacheData.startTask(taskId, stepId))
                throw new Exception("当前任务不能开启，没有cachedata，taskId：" + taskId);
            if (!_conditionMeetHelper.startTask(taskId))
                throw new Exception("当前任务不能开启，没有condition配置，taskId:" + taskId);
            _curTaskId = taskId;
            _stepController.startStep(_curTaskId, stepId);
            checkTaskUI();
        }

        public void finishStep(int taskId, int stepId)
        {
            if (taskId != curTaskId)
                throw new Exception("finishStep与当前进行任务不符的taskId：" + taskId + ",当前进行的任务为：" + curTaskId);

            if (!_taskCacheData.finishStep(taskId, stepId))
                throw new Exception("finishTask当前任务" + taskId + "还未开启，不能完成");
            _stepController.finishStep();
            _conditionMeetHelper.finishStep(stepId);
            checkTaskUI();
            _stepController.checkFinishTask();
        }

        public void finishTask()
        {
            finishTask(_curTaskId);
        }

        public void finishTask(int taskId)
        {
            if (!_taskCacheData.finishTask(taskId))
                throw new Exception("finishTask当前任务" + taskId + "还未开启，不能完成");
            if (!_conditionMeetHelper.finishTask(taskId))
                throw new Exception("finishTask当前任务" + taskId + "condition不匹配，不能完成");
            _curTaskId = 0;
            _conditionMeetHelper.updateCondition(_taskCacheData.canConductTaskList);
            checkTaskUI();
        }

        public void resetTask(int taskId)
        {
            HasActionObjectManager.Instance.npcManager.RemoveObjBeside(-1);
            _curTaskId = 0;
            _stepController.curStepId = 0;
            _stepController.curStepId = _stepController.curStepId < 0 ? 0 : _stepController.curStepId;
            _taskCacheData.resetTask(taskId);
            _conditionMeetHelper.dispose();
            _conditionMeetHelper.updateCondition(_taskCacheData.canConductTaskList);
            checkTaskUI();
        }

        public bool checkInTask(int taskId)
        {
            return _curTaskId == taskId;
        }

        public bool checkCanDoTask(int taskId)
        {
            return _taskCacheData.canConductTaskList.ContainsKey(taskId);
        }

        private void checkTaskUI()
        {
            if (curStepId != 0 &&
                MTBTaskController.Instance.conditionMeetHelper != null &&
                MTBTaskController.Instance.conditionMeetHelper.curStepCondition() != null)
                UIManager.Instance.showUI<TaskUI>(UITypes.TASK, TaskUIPath);
            else
                UIManager.Instance.closeUI(UITypes.TASK);
        }

        public void dispose()
        {
            _conditionMeetHelper.dispose();
            _conditionMeetHelper = null;
            _taskCacheData = null;
            _stepController = null;
        }
    }
}
