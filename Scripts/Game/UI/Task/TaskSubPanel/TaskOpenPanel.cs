using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
    public class TaskOpenPanel : UISubPanel
    {
        private Transform _selectParentTrans;
        private List<GameObject> _textList;
        private Button _closeButton;

        public override void InitView()
        {
            base.InitView();
            _selectParentTrans = this.transform.Find("TextView").transform.Find("TextContainer").transform;
            _closeButton = this.transform.Find("ButtonBack").GetComponent<Button>();
            _textList = new List<GameObject>();
        }

        public override void Open()
        {
            base.Open();
            _textList.Clear();
            List<string> tips = MTBTaskController.Instance.conditionMeetHelper.curTaskTips;
            for (int i = 0; i < tips.Count; i++)
            {
                if (tips[i] == null || tips[i] == "" || tips[i] == "0")
                {
                    UIManager.Instance.closeUI(UITypes.TASK);
                    return;
                }
                GameObject obj = GameObject.Instantiate(Resources.Load("UI/Common/TextField") as GameObject);
                obj.transform.SetParent(_selectParentTrans);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.GetComponent<Text>().text = tips[i];
                _textList.Add(obj);
            }
        }

        public override void Close()
        {
            removeEvents();

            for (int i = 0; i < _textList.Count; i++)
            {
                GameObject text = _textList[i];
                text.transform.SetParent(null);
                Destroy(text);
            }
            _textList.Clear();
            base.Close();
        }

        protected override void InitEvents()
        {
            _closeButton.onClick.AddListener(delegate()
            {
                removeEvents();
                mainPanel.OpenSubPanel(1);
                mainPanel.CloseSubPanel(this.panelId);
            });
        }

        protected override void removeEvents()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveAllListeners();
        }

    }
}
