/******
 * UI层级管理控制
 * 
 * *****/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class UILayerController
    {

        private Dictionary<UITypes, UIOperateBase> _UIList = new Dictionary<UITypes, UIOperateBase>();
        private Dictionary<UITypes, int> _UILayerList = new Dictionary<UITypes, int>();

        public void register(UIOperateBase ui)
        {
            if (!_UIList.ContainsKey(ui.uiType))
            {
                _UIList.Add(ui.uiType, ui);
            }
        }

        public void updateLayer(UITypes type, int layers)
        {
            if (!_UILayerList.ContainsKey(type))
                _UILayerList.Add(type, layers);
            else
                _UILayerList[type] = layers;
            if (_UIList.ContainsKey(type))
                _UIList[type].updateLayer(layers);
        }
    }

    public class UILayers
    {
        public static int HIGHEST = 5;
        public static int HIGHT = 4;
        public static int MIDDLE = 3;
        public static int LOW = 2;
        public static int LOWEST = 1;
    }
}
