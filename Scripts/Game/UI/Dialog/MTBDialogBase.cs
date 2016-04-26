using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
namespace MTB
{
    public class MTBDialogBase : UIOperateBase
    {
        protected GameObject _attachObj;

        protected float _dialogeTime;
        protected int _stepId;
        protected Text _txt;
        protected IconComponent _iconComponent;
        protected Dictionary<int, string> _npcId;
        protected MTBDialogueData _dialogData;
        protected MTBDialogueStepData _dialogStepData;
        protected bool _dialogOpenMark;
        private string _iconResPath = "UI/Icon/NpcIcons/";
        private TaskPanelController _controller;

        public void setTaskPanelController(TaskPanelController controller)
        {
            _controller = controller;
        }

        public override void Init(params object[] paras)
        {
            _dialogOpenMark = false;
            uiType = UITypes.DIALOG;
            base.Init(paras);
            GameObject icons = GameObject.Find("DialogIconContainer");
            _txt = GameObject.Find("DialogText").GetComponent<Text>();
            _iconComponent = icons.AddComponent<IconComponent>();
            _iconComponent.initComponents("DialogIconContainer", uiType);
        }

        void Update()
        {
            if (!_dialogOpenMark)
                return;
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                //#if IPHONE || ANDROID
                //                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                //#else
                //                if (EventSystem.current.IsPointerOverGameObject())
                //#endif
                clickDialog();
            }
            updateDialogPosition();
        }

        protected virtual void updateDialogPosition()
        {

        }

        public virtual void startDialog(MTBDialogueData data)
        {
            _dialogData = data;
            _stepId = 1;
            showNextStep(_stepId);
        }

        protected virtual void finishDialog()
        {
            closeDialog();
            if (_controller != null)
            {
                _controller.finishTaskDialog();
            }
        }

        protected virtual void showNextStep(int stepId)
        {
            _dialogOpenMark = true;
            _dialogStepData = _dialogData.dialogueList[stepId];
            _iconComponent.setIconId(_dialogStepData.npcId);
            _txt.text = _dialogStepData.content;
            Open();
        }

        protected virtual void clickDialog()
        {
            _dialogOpenMark = false;
            if (_dialogStepData.next == "end")
            {
                finishDialog();
                return;
            }
            _stepId = Convert.ToInt32(_dialogStepData.next);
            closeDialog();
            showNextStep(_stepId);
        }

        protected virtual void closeDialog()
        {
            _iconComponent.removeIcon();
            _txt.text = "";
            Close();
        }

        protected virtual void onOverTime()
        {
            Close();
        }
    }
}
