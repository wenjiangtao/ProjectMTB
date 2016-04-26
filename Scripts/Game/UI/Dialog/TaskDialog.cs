using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
namespace MTB
{
    public class TaskDialog : MTBDialogBase
    {
        private TaskPanelController _controller;

        public void setTaskPanelController(TaskPanelController controller)
        {
            _controller = controller;
        }

        protected override void finishDialog()
        {
            closeDialog();
            _controller.finishTaskDialog();
        }
    }
}
