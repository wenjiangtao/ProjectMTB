using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
    public class TaskClosePanel : UISubPanel
    {
        private Button _button;
        private Image _shakeImg;
        private float _shakeTime;
        private bool _openMark;

        public override void InitView()
        {
            base.InitView();
            _button = this.transform.Find("Button").GetComponent<Button>();
            _shakeImg = _button.transform.Find("Image").transform.Find("ShakeImg").GetComponent<Image>();
        }

        public override void Open()
        {
            base.Open();
            _shakeTime = 0;
            _openMark = true;
        }

        public override void Close()
        {
            removeEvents();
            _shakeTime = 0;
            _openMark = false;
            base.Close();
        }

        protected override void InitEvents()
        {
            _button.onClick.AddListener(delegate()
            {
                removeEvents();
                mainPanel.OpenSubPanel(2);
                mainPanel.CloseSubPanel(this.panelId);
            });
        }

        protected override void removeEvents()
        {
            if (_button != null)
                _button.onClick.RemoveAllListeners();
        }

        void Update()
        {
            if (_openMark)
            {
                _shakeTime += Time.deltaTime;
                if (_shakeTime >= 0.5f)
                    _shakeImg.enabled = false;

                else
                    _shakeImg.enabled = true;

                if (_shakeTime >= 1f)
                    _shakeTime = 0;
            }
        }
    }
}
