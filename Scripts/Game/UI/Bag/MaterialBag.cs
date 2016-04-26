using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class MaterialBag : UIOperateBase
    {
        private int _id;

        private IBagComponent[] _components;

        public override void Init(params object[] paras)
        {
            uiType = UITypes.MAIN_BAG;
            _id = 0;
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            viewGo.AddComponent<BagIconsComponent>();
            viewGo.AddComponent<BagTabsComponent>();
            viewGo.AddComponent<BagInfoComponent>();
            _components = new IBagComponent[3];
            _components[0] = viewGo.GetComponent<BagIconsComponent>();
            _components[1] = viewGo.GetComponent<BagTabsComponent>();
            _components[2] = viewGo.GetComponent<BagInfoComponent>();
            //Transform bagTrans = viewGo.transform.FindChild("Bag");
            foreach (IBagComponent comp in _components)
            {
                comp.initComponents(uiType);
            }
        }

        public override void Open()
        {
            base.Open();
            foreach (IBagComponent comp in _components)
            {
                comp.refresh();
            }
            GameObject.Find("BagViewScrollBar").GetComponent<Scrollbar>().value = 1;
        }

        protected override void InitEvents()
        {
            GameObject.Find("Bagclose").GetComponent<Button>().onClick.AddListener(delegate()
            {
                UIManager.Instance.closeUI(uiType);
            });
            base.InitEvents();
        }

        public override void Dispose()
        {
            //foreach (IBagComponent comp in _components)
            //{
            //    comp.dispose();
            //}
            base.Dispose();
        }
    }
}
