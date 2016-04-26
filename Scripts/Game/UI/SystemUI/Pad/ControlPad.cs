using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MTB
{
    public class ControlPad : UIPad
    {
        private Scrollbar _bigJoyScrollbar;
        private Scrollbar _crossModeScrollbar;
        private Scrollbar _leftModeScrollbar;

        private float _curCrossMode;

        protected override void InitComponents()
        {
            //todo
            _bigJoyScrollbar = GameObject.Find("BigJoyScrollbar").GetComponent<Scrollbar>();
            _crossModeScrollbar = GameObject.Find("CrossModeScrollbar").GetComponent<Scrollbar>();
            _leftModeScrollbar = GameObject.Find("LeftModeScrollbar").GetComponent<Scrollbar>();

            _curCrossMode = _crossModeScrollbar.value;
            initEvent();
        }

        private void initEvent()
        {
            _bigJoyScrollbar.onValueChanged.AddListener(onChangeJoySize);
            _crossModeScrollbar.onValueChanged.AddListener(onChangeCrossMode);
            _leftModeScrollbar.onValueChanged.AddListener(onChangeLeftMode);
        }

        private void removeEvent()
        {
            _bigJoyScrollbar.onValueChanged.RemoveAllListeners();
            _crossModeScrollbar.onValueChanged.RemoveAllListeners();
            _leftModeScrollbar.onValueChanged.RemoveAllListeners();
        }

        private void onChangeJoySize(float value)
        {

        }

        private void onChangeCrossMode(float value)
        {
            if (_curCrossMode != value)
            {
                _curCrossMode = value;
                EventManager.SendEvent(EventMacro.ON_CLICK_SWITCH_PROPMODEL, value);
            }
        }

        private void onChangeLeftMode(float value)
        {

        }
    }
}
