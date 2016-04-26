using System;
using UnityEngine;
namespace MTB
{
    public class TaskPanelController
    {
        private int _curTaskId;
        private int _curStepId;
        private int _dialogId;
        private MTBDialogBase _dialog;
        private MTBTaskData _curTaskData;

        public TaskPanelController()
        {
            _dialog = UIManager.Instance.getUI(UITypes.DIALOG) as MTBDialogBase;
            _dialog.setTaskPanelController(this);
        }

        public void setCurTask(MTBTaskData taskData, int stepId)
        {
            _curTaskId = taskData.id;
            _curStepId = stepId;
            _curTaskData = taskData;
        }

        public void startTaskDialog(int dialogId)
        {
            _dialogId = dialogId;
            MTBDialogueData dialogueData = MTBDialogueDataManager.Instance.getData(_curTaskId, _dialogId);
            _dialog.startDialog(dialogueData);
        }

        public void finishTaskDialog()
        {
            EventManager.SendEvent(PlotEvent.DIALOGEFINISH, _curTaskId, _curStepId);
        }
    }
}
