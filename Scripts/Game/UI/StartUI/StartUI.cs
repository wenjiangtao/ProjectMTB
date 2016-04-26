using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MTB
{
    public class StartUI : UIOperateBase
    {
        private Button _startBtn;
        private Button _systemBtn;

        public override void Init(params object[] paras)
        {
            uiType = UITypes.START;
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            _startBtn = GameObject.Find("StartBtn").GetComponent<Button>();
            _systemBtn = GameObject.Find("system").GetComponent<Button>();
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            _startBtn.onClick.AddListener(delegate()
            {
                UIManager.Instance.showUI<StartGameUI>(UITypes.START_GAME);
                UIManager.Instance.closeUI(uiType);
            });
            _systemBtn.onClick.AddListener(delegate()
            {
                UIManager.Instance.showUI<SetUpUI>(UITypes.SETUP);
            });
        }

        protected override void removeEvents()
        {
            base.removeEvents();
            _systemBtn.onClick.RemoveAllListeners();
            _startBtn.onClick.RemoveAllListeners();
        }
    }
}
