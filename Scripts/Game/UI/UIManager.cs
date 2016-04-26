/**
 * 提供所有UI的开启和关闭的方法
 * UI也可自行处理关闭
 * 在开关某UI时需要进行其他处理可以使用抛事件带UItype的方法
 * 如果无其他处理可直接调show或close
 * 
 * **/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class UIManager : Singleton<UIManager>
    {
        private static Dictionary<UITypes, UIOperateBase> _UIList = new Dictionary<UITypes, UIOperateBase>();

        private UILayerController _layerController;

        private int _openCount;

        private void addEventListener()
        {
        }

        void Awake()
        {
            _layerController = new UILayerController();
            GameObject root = GameUIRoot.RootGo;
            _openCount = 0;
            registerUIs();
        }

        private void registerUIs()
        {
            registerUI<StartUI>(UITypes.START);
            registerUI<StartGameUI>(UITypes.START_GAME);
            registerUI<SelectWorldUI>(UITypes.SELECTWORLD);
            registerUI<LoadingUI>(UITypes.LOADING);
            registerUI<MainUI>(UITypes.MAIN_UI);
            registerUI<MaterialBag>(UITypes.MAIN_BAG);
            registerUI<SetUpUI>(UITypes.SETUP);
            registerUI<Cross>(UITypes.CROSS);
            registerUI<MTBDialogBase>(UITypes.DIALOG);
            registerUI<LoginUI>(UITypes.LOGIN);
            registerUI<RegisterMainUI>(UITypes.REGISTER);
            registerUI<UserInfoUI>(UITypes.USERINFO);
            registerUI<PasswordFoundUI>(UITypes.PASSWORDFOUND);
            MTBUserInput.Instance.SetJoyStickActive(false);
        }

        public void registerUI<T>(UITypes type) where T : UIOperateBase
        {
            if (!checkNameConfig(type))
                return;

            string name = UIName.Type2Name[type];
            _UIList.Add(type, UIOperateBase.New<T>(name));
            _layerController.register(_UIList[type]);
        }

        //动态增加ui时需传入uipath，在uitype和uiname中均需要配置
        public UIOperateBase showUI<T>(UITypes type, string path = "") where T : UIOperateBase
        {
            if (!_UIList.ContainsKey(type))
            {
                if (!checkNameConfig(type))
                    return null;
                string name = UIName.Type2Name[type];
                if (path == "")
                    _UIList.Add(type, UIOperateBase.New<T>(name));
                else
                    _UIList.Add(type, UIOperateBase.DynamicNew<T>(name, path));
            }
            _UIList[type].Open();
            if (!checkIsHUDUI(type))
                updateOpenCout(1);
            EventManager.SendEvent(EventMacro.SHOW_UI, type);
            _layerController.updateLayer(type, UILayers.HIGHEST);
            return _UIList[type];
        }

        public UIOperateBase getUI(UITypes type)
        {
            if (!_UIList.ContainsKey(type))
                throw new Exception("非法的UI类型!");
            return _UIList[type];
        }

        public void closeUI(UITypes type)
        {
            if (!_UIList.ContainsKey(type))
                return;
            _UIList[type].Close();
            if (!checkIsHUDUI(type))
                updateOpenCout(-1);
            _layerController.updateLayer(type, UILayers.LOWEST);
            EventManager.SendEvent(EventMacro.CLOSE_UI, type);
        }

        private void updateOpenCout(int value)
        {
            _openCount += value;
            _openCount = _openCount <= 0 ? 0 : _openCount;
            UIKeyboard.setEnable(true);
            MTBKeyboard.setEnable(false);

            if (_openCount <= 1 && _UIList.ContainsKey(UITypes.MAIN_UI))
            {
                UIKeyboard.setEnable(false);
                MTBKeyboard.setEnable(true);
            }
        }

        private bool checkNameConfig(UITypes type)
        {
            if (!UIName.Type2Name.ContainsKey(type))
            {
                Debug.LogError("没有当前UI的配置！请在UIName中配置UI！");
                return false;
            }
            return true;
        }

        private bool checkIsHUDUI(UITypes type)
        {
            return type == UITypes.TASK;
        }

    }
}
