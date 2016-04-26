using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MTB
{
    public class SystemPad : UIPad
    {
        private Scrollbar _thirdPersonScrollbar;
        private Scrollbar _brightnessScrollbar;
        private Scrollbar _volumnScrollbar;
		private Scrollbar _multiScrollbar;
		private Button _quitButton;
        private float _curViewValue = 0;
		private float _curMultiValue = 1;

        protected override void InitComponents()
        {
            _volumnScrollbar = GameObject.Find("VolumeScrollbar").GetComponent<Scrollbar>();
            _brightnessScrollbar = GameObject.Find("BrightnessScrollbar").GetComponent<Scrollbar>();
            _thirdPersonScrollbar = GameObject.Find("ThirdScrollbar").GetComponent<Scrollbar>();
			_multiScrollbar = GameObject.Find("MultiScrollbar").GetComponent<Scrollbar>();
			_quitButton = GameObject.Find("QuitGameButton").GetComponent<Button>();
            _brightnessScrollbar.value = 0.5f;
			_multiScrollbar.value = _curMultiValue;
            initEvent();
        }

        private void initEvent()
        {
            _thirdPersonScrollbar.onValueChanged.AddListener(onThirdPersonViewChange);
            _brightnessScrollbar.onValueChanged.AddListener(onBrightnessChange);
            _volumnScrollbar.onValueChanged.AddListener(onVolumnChange);
			_multiScrollbar.onValueChanged.AddListener(onMultiChange);
			_quitButton.onClick.AddListener(onQuitGame);
        }

        private void removeEvent()
        {
            _thirdPersonScrollbar.onValueChanged.RemoveAllListeners();
            _brightnessScrollbar.onValueChanged.RemoveAllListeners();
            _volumnScrollbar.onValueChanged.RemoveAllListeners();
			_multiScrollbar.onValueChanged.RemoveAllListeners();
			_quitButton.onClick.RemoveAllListeners();
        }

		private void onQuitGame()
		{
			Debug.Log("退出游戏!");
			Application.Quit();
		}

        private void onThirdPersonViewChange(float value)
        {
            if (value != _curViewValue)
            {
                EventManager.SendEvent(EventMacro.ON_CLICK_SWITCH_VIEW);
                _curViewValue = value;
            }
        }

        private void onBrightnessChange(float value)
        {
            //HasActionObjectManager.Instance.monsterManager.setLight(value * 3 / 2 + 0.5f);
            //Color c = _clasicC;
            //c *= (value + 0.5f);
            //RenderSettings.ambientLight = Color.red;
            //Debug.Log(RenderSettings.ambientLight.r + " " + RenderSettings.ambientLight.g + " " + RenderSettings.ambientLight.b + " " + RenderSettings.ambientLight.a);
        }

        private void onVolumnChange(float value)
        {

        }

		private void onMultiChange(float value)
		{
			if(value != _curMultiValue)
			{
				_curMultiValue = value;
				if(value == 0)
				{
					NetManager.Instance.StartServer();
				}
				else
				{
					NetManager.Instance.StopServer();
				}
			}
		}

        public override void UnRegister()
        {
            removeEvent();
            base.UnRegister();
        }
    }
}
