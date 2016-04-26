using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MTB
{
    public class SetUpUI : UIOperateBase
    {
        private TabComponent _tabComponent;
        private SystemPadComponent _padComponent;

        public override void Init(params object[] paras)
        {
            uiType = UITypes.SETUP;
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            _tabComponent = gameObject.AddComponent<TabComponent>();
            _tabComponent.initComponents(4, "SetUpTab", "SetUpselect", uiType);

            _padComponent = gameObject.AddComponent<SystemPadComponent>();
            _padComponent.initComponents(4, "SetUpPad", uiType);
        }

        protected override void InitEvents()
        {
            GameObject.Find("SetUpclose").GetComponent<Button>().onClick.AddListener(delegate()
            {
                UIManager.Instance.closeUI(uiType);
            });
            base.InitEvents();
        }

        protected override void removeEvents()
        {
            base.removeEvents();
            _tabComponent.dispose();
            _padComponent.dispose();
        }
    }
}
