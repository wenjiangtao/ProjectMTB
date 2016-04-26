using UnityEngine;
using System.Collections;
namespace MTB
{
    public class BagTabsComponent : MonoBehaviour, IBagComponent
    {
        private GameObject[] _normalTabList;
        private GameObject[] _activeTabLIst;
        private TabComponent _tabComponent;

        public void initComponents(params object[] paras)
        {
            _tabComponent = gameObject.AddComponent<TabComponent>();
            _tabComponent.initComponents(BagConfig.TAB_SUM, "BagTab", "Bagselect", (UITypes)paras[0]);
        }

        public void refresh()
        {
            resetTabs();
        }

        public void resetTabs()
        {
            _tabComponent.resetTab();
        }
        public void dispose() { }
    }
}

