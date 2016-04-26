using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LoadingUI : UIOperateBase
    {
        private GameObject _loadingBar;
        private GameObject _loadingEffect;
        private string _processEvt;
        private string _finishEvt;
        private bool _isLoaded;
        private float _processCount;
        private float _percentCount;
        private float _percentLen;

        public override void Init(params object[] paras)
        {
            _isLoaded = false;
            uiType = UITypes.LOADING;
            base.Init(paras);
            //InitEvents();
        }

        public override void InitView()
        {
            base.InitView();
            _loadingBar = GameObject.Find("LoadingBar");
            _loadingEffect = GameObject.Find("LoadingEffect");
            _loadingBar.transform.localPosition = new Vector3(-370, 0, 0);
        }

        public void setProcessEvt(string evt, float count)
        {
            _processEvt = evt;
            _processCount = count;
            _percentCount = 0;
            _percentLen = (float)(370 / count);
            EventManager.RegisterEvent(_processEvt, onProcess);
        }

        public void setFinishEvt(string evt)
        {
            _finishEvt = evt;
            EventManager.RegisterEvent(_finishEvt, onLoadingFinish);
        }

        public void setLoadedEvt(string evt)
        {
        }

        public override void Close()
        {
            if (_finishEvt != null)
                EventManager.UnRegisterEvent(_finishEvt, onLoadingFinish);
            if (_processEvt != null)
                EventManager.UnRegisterEvent(_processEvt, onProcess);
			EventManager.SendEvent(EventMacro.LOADING_UI_FINISH);
            base.Close();
        }

        private void onLoadingFinish(params object[] paras)
        {
            _isLoaded = true;
			UIManager.Instance.closeUI(uiType);
        }

        private void onProcess(params object[] paras)
        {
            _percentCount++;
			if(_percentCount > _processCount)_percentCount = _percentCount;
			else
           	 _loadingBar.transform.localPosition = new Vector3(_loadingBar.transform.localPosition.x + _percentLen, 0, 0);
		}
    }
}
