using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class SelectComponent : MonoBehaviour, IUIComponent
    {
        private UITypes _type;
        private List<SelectItem> _itemList;
        private List<GameObject> _selectView;

        public void initComponents(params object[] paras)
        {
            _type = (UITypes)paras[0];
            _itemList = new List<SelectItem>();
            _selectView = new List<GameObject>();
        }

        public void addComponent(params object[] paras)
        {
            if (_itemList == null)
                _itemList = new List<SelectItem>();
            int i = (int)paras[0];
            _selectView.Add(GameObject.Find("World" + (i + 1)));
            NetType netType = (NetType)paras[1];
            if (netType == NetType.Single)
            {
                SelectItem item = _selectView[i].AddComponent<SingleSelectItem>();
                item.Init(i.ToString(), _type, paras[2], paras[3], paras[4], paras[5]);
                _itemList.Add(item);
            }
            else
            {
                SelectItem item = _selectView[i].AddComponent<NetSelectItem>();
                item.Init(i.ToString(), _type, paras[2], paras[3]);
                _itemList.Add(item);
            }

        }

        public int getComponentNum()
        {
            return _selectView.Count;
        }

        public void dispose()
        {
        }
    }
}
