using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
    public class TaskUI : UIMainPanel
    {
        public override void Init(params object[] paras)
        {
            uiType = UITypes.TASK;
            base.Init(paras);
        }
        protected override void InitSubPanel()
        {
            RegisterSubPanel<TaskClosePanel>(1, "Packup");
            RegisterSubPanel<TaskOpenPanel>(2, "Packout");
        }

        public override void Open()
        {
            base.Open();
            CloseSubPanel(2);
            OpenSubPanel(2);
            CloseSubPanel(1);
        }
        public override void Close()
        {
            CloseSubPanel(1);
            CloseSubPanel(2);
            base.Close();
        }
    }
}
