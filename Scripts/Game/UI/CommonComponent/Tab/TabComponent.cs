/******
 * 此公共组件供面板tab切换和显示，与PadComponent配套使用
 * 使用示例参阅SetUpUI
 * ******/
using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class TabComponent : MonoBehaviour, IUIComponent
    {
        protected GameObject[] _tabList;
        protected UITypes _type;
        protected string _selectMcName;

        public void initComponents(params object[] paras)
        {
            int tabNum = Convert.ToInt32(paras[0]);
            string tabName = Convert.ToString(paras[1]);
            _selectMcName = Convert.ToString(paras[2]);
            _type = (UITypes)paras[3];
            int index;
            _tabList = new GameObject[tabNum];
            for (int i = 0; i < tabNum; i++)
            {
                index = i + 1;
                _tabList[i] = GameObject.Find(tabName + index);
                addTabScript(_tabList[i], index);
            }
            resetTab();
        }

        protected virtual void addTabScript(GameObject tab, int id)
        {
            tab.AddComponent<UITab>();
            tab.GetComponent<UITab>().Init(id.ToString(), _type, _selectMcName);
        }

        public void resetTab()
        {
            foreach (GameObject tab in _tabList)
            {
                tab.GetComponent<UITab>().setSelect(false);
            }
            _tabList[0].GetComponent<UITab>().setSelect(true);
        }

        public void dispose()
        {

        }
    }
}
