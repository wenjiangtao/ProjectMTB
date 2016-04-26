using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
namespace MTB
{
    public class BagIconsComponent : MonoBehaviour, IBagComponent
    {
        private UITypes _uiType;
        private string _resPath = "UI/Icon/ItemIcons/";
        private IconsComponent _iconComponent;

        public void initComponents(params object[] paras)
        {
            _uiType = (UITypes)paras[0];
            GameObject icons = GameObject.Find("BagIcons");
            _iconComponent = icons.AddComponent<IconsComponent>();
            _iconComponent.initComponents("BagButton", _uiType, _resPath, "Bagselect");
        }

        public void refresh()
        {
            _iconComponent.showIcon(0);
        }

        public void dispose() { }
    }
}

