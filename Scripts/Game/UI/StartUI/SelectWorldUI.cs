using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class SelectWorldUI : UIOperateBase
    {
        private SelectComponent _selectCom;
        private Button _createButton1;
        private Button _createButton2;
        private Transform _selectParentTrans;
        private Transform _newInfoTrans;

        public override void Init(params object[] paras)
        {
            uiType = UITypes.SELECTWORLD;
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            _selectParentTrans = GameObject.Find("SelectItemContainer").transform;
            _newInfoTrans = this.transform.Find("Panel/New");
            _newInfoTrans.gameObject.SetActive(false);
            _selectCom = gameObject.AddComponent<SelectComponent>();
            _selectCom.initComponents(uiType);
            List<WorldFileInfo> infos = WorldPersistanceManager.Instance.GetAllWorldInfo();
            for (int i = 0; i < infos.Count; i++)
            {
                addNewSelectItem();
                _selectCom.addComponent(_selectCom.getComponentNum(), NetType.Single,
                                        infos[i].worldConfigStr, infos[i].worldName, infos[i].lastSaveTimeStr, infos[i].seed);
            }
            _createButton1 = GameObject.Find("CreateWorld").GetComponentInChildren<Button>();
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            _createButton1.onClick.AddListener(delegate()
            {
                addNewWorld();
            });
            EventManager.RegisterEvent(UIEventMacro.CLICK_ITEM, onSelectWorld);
            EventManager.RegisterEvent(NetEventMacro.ON_SEARCH_SERVER_SIGNAL, OnSearchServerSignal);

            NetManager.Instance.broadcast.StartBroadcast();
            NetManager.Instance.broadcast.SendPackage(PackageFactory.GetPackage(PackageType.RequestServerSignal));
        }

        protected override void removeEvents()
        {
            base.removeEvents();
            _createButton1.onClick.RemoveAllListeners();
            if (_createButton2 != null)
                _createButton2.onClick.RemoveAllListeners();
            EventManager.UnRegisterEvent(UIEventMacro.CLICK_ITEM, onSelectWorld);
            EventManager.UnRegisterEvent(NetEventMacro.ON_SEARCH_SERVER_SIGNAL, OnSearchServerSignal);
            NetManager.Instance.broadcast.StopBroadcast();
        }

        private void OnSearchServerSignal(object[] param)
        {
            string ip = (string)param[0];
            int port = (int)param[1];
            addNewSelectItem();
            _selectCom.addComponent(_selectCom.getComponentNum(), NetType.Local, ip, port);
            Debug.Log("附近有玩家，其ip:" + ip + " port:" + port + " index" + _selectCom.getComponentNum());
        }

        private void addNewSelectItem()
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("UI/Common/WorldSelect") as GameObject);
            obj.name = "World" + (_selectCom.getComponentNum() + 1);
            obj.transform.SetParent(_selectParentTrans);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
        }

        private void addNewWorld()
        {
            _selectParentTrans.gameObject.SetActive(false);
            _newInfoTrans.gameObject.SetActive(true);
            InputField worldNameTxt = _newInfoTrans.Find("WorldNameTxt").GetComponent<InputField>();
            _createButton2 = _newInfoTrans.Find("CreatBtn").GetComponent<Button>();
            _createButton2.onClick.AddListener(delegate()
            {
                onNewWorldSet(worldNameTxt.text);
            });
        }

        private void onNewWorldSet(string worldName)
        {
            string timeStr = "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            UIManager.Instance.closeUI(uiType);
            LoadingUI loadingUI = UIManager.Instance.showUI<LoadingUI>(UITypes.LOADING) as LoadingUI;
            loadingUI.setProcessEvt(EventMacro.CHUNK_GENERATE_FINISH, 121);
            loadingUI.setFinishEvt(EventMacro.GENERATE_FIRST_WORLD_FINISH);
            EventManager.SendEvent(EventMacro.SELECT_WORLD, NetType.Single, GameConfig.Instance.defaultWorldConfigStr, worldName + timeStr, GameConfig.Instance.defalutSeed);
        }

        private void onSelectWorld(params object[] paras)
        {
            if ((UITypes)paras[0] == uiType)
            {
                if ((NetType)paras[1] == NetType.Single)
                {
                    UIManager.Instance.closeUI(uiType);
                    LoadingUI loadingUI = UIManager.Instance.showUI<LoadingUI>(UITypes.LOADING) as LoadingUI;
                    loadingUI.setProcessEvt(EventMacro.CHUNK_GENERATE_FINISH, 121);
                    loadingUI.setFinishEvt(EventMacro.GENERATE_FIRST_WORLD_FINISH);
                    EventManager.SendEvent(EventMacro.SELECT_WORLD, NetType.Single, paras[2], paras[3], paras[4]);
                }
                else
                {
                    EventManager.RegisterEvent(NetEventMacro.ON_LINK_SERVER, OnLinkServer);
                    NetManager.Instance.client.Connect((string)paras[2], (int)paras[3]);
                }
            }
        }
        private void OnLinkServer(object[] param)
        {
            EventManager.UnRegisterEvent(NetEventMacro.ON_LINK_SERVER, OnLinkServer);
            Debug.Log("连接服务器成功!");
            EventManager.RegisterEvent(NetEventMacro.ON_JION_GAME, OnJoinGame);
            NetManager.Instance.client.JoinGame(1);
        }

        private void OnJoinGame(object[] param)
        {
            EventManager.UnRegisterEvent(NetEventMacro.ON_JION_GAME, OnJoinGame);
            PlayerInfo info = (PlayerInfo)param[0];
            Debug.Log("进入游戏成功!isMe:" + info.isMe);
            if (!info.isMe) return;
            string worldConfigName = (string)param[1];
            int seed = (int)param[2];
            float worldTime = (float)param[3];
            UIManager.Instance.closeUI(uiType);
            LoadingUI loadingUI = UIManager.Instance.showUI<LoadingUI>(UITypes.LOADING) as LoadingUI;
            loadingUI.setProcessEvt(EventMacro.CHUNK_GENERATE_FINISH, 121);
            loadingUI.setFinishEvt(EventMacro.GENERATE_FIRST_WORLD_FINISH);
            EventManager.SendEvent(EventMacro.SELECT_WORLD, NetType.Local, worldConfigName, "", seed, info, worldTime);
        }
    }
}
