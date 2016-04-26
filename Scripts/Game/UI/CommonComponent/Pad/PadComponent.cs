/******
 * 此公共组件供面板主要逻辑区域的切换和显示，与TabComponent配套使用
 * 只适用一次显示单个区域，不适用于多个区域同时显示
 * 需要自行实现部分功能
 * 使用示例参阅SetUpUI
 * ******/
using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class PadComponent : MonoBehaviour, IUIComponent
    {
        protected GameObject[] _padList;
        protected UITypes _type;

        public void initComponents(params object[] paras)
        {
            int padNum = Convert.ToInt32(paras[0]);
            string padName = Convert.ToString(paras[1]);
            _type = (UITypes)paras[2];
            int index;
            _padList = new GameObject[padNum];
            for (int i = 0; i < padNum; i++)
            {
                index = i + 1;
                _padList[i] = GameObject.Find(padName + index);
                addPadScript(_padList[i], index);
            }
        }

        //需要重写以加载不同的脚本
        protected virtual void addPadScript(GameObject pad, int id)
        {
            Debug.LogError("addPadScript方法需重写！");
            pad.AddComponent<UIPad>();
            pad.GetComponent<UIPad>().Init(id.ToString(), _type);
            if (id == 1)
            {
                pad.GetComponent<UIPad>().setSelect(true);
            }
        }

        public void dispose()
        {

        }
    }
}
