using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class StartGameUI : UIOperateBase
    {
        private Button _singleStartBtn;
        private Button _loadArchiveBtn;
        private Button _loginEntranceBtn;
        private Button _systemBtn;

        public override void Init(params object[] paras)
        {
            uiType = UITypes.START_GAME;
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            _singleStartBtn = this.transform.Find("Panel/LoginBtn").GetComponent<Button>();
            _loadArchiveBtn = this.transform.Find("Panel/LoginBtn2").GetComponent<Button>();
            _loginEntranceBtn = this.transform.Find("Panel/LoginEntrance").GetComponent<Button>();
            _systemBtn = this.transform.Find("Panel/SystemBtn").GetComponent<Button>();
        }

        public override void Open()
        {
            base.Open();
            if (UserService.Instance.isLogin)
            {
                _loginEntranceBtn.transform.Find("Text").GetComponent<Text>().text = UserService.Instance.userInfo.loginName;
            }
            else
            {
                _loginEntranceBtn.transform.Find("Text").GetComponent<Text>().text = "百田账号";
            }
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            _singleStartBtn.onClick.AddListener(delegate()
            {
                addNewNewBieWorld();
            });
            _loadArchiveBtn.onClick.AddListener(delegate()
            {
                UIManager.Instance.showUI<SelectWorldUI>(UITypes.SELECTWORLD);
                UIManager.Instance.closeUI(uiType);
            });
            _loginEntranceBtn.onClick.AddListener(delegate()
            {
                UIManager.Instance.showUI<LoginUI>(UITypes.LOGIN);
                UIManager.Instance.closeUI(uiType);
            });
            _systemBtn.onClick.AddListener(delegate
            {
                if (UserService.Instance.isLogin)
                {
                    UIManager.Instance.showUI<UserInfoUI>(UITypes.USERINFO);
                    UIManager.Instance.closeUI(uiType);
                }
                else
                {
                    UnityEngine.Debug.Log("用户未登陆!");
                }
            });
        }

        protected override void removeEvents()
        {
            base.removeEvents();
            _singleStartBtn.onClick.RemoveAllListeners();
            _loadArchiveBtn.onClick.RemoveAllListeners();
            _loginEntranceBtn.onClick.RemoveAllListeners();
            _systemBtn.onClick.RemoveAllListeners();
        }

        //暂时把新建新手场景的方法写在这里
        private void addNewNewBieWorld()
        {
            string timeStr = "-" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            string path = GameConfig.Instance.WorldSavedPath + "/" + GameConfig.Instance.defaultWorldConfigStr + "_" + "剧情模式" + timeStr + "_" + GameConfig.Instance.defalutSeed;
            DirectoryInfo dd = new DirectoryInfo(path);
            if (!dd.Exists)
            {
                dd.Create();
            }


            UnityEngine.Object[] objlist = Resources.LoadAll("Data/Plot/SceneData/Newbie");
            for (int i = 0; i < objlist.Length; i++)
            {
                if (objlist[i] is TextAsset)
                {
                    //string objstr = objlist[i].ToString();
                    byte[] bt = (objlist[i] as TextAsset).bytes;
                    FileInfo newf = new FileInfo(path + "/" + objlist[i].name + ".mtb");
                    FileStream fs = newf.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    //byte[] byteArray = System.Text.Encoding.Default.GetBytes(objstr);
                    fs.Write(bt, 0, bt.Length);
                    fs.Close();
                }
            }

            UIManager.Instance.closeUI(uiType);
            LoadingUI loadingUI = UIManager.Instance.showUI<LoadingUI>(UITypes.LOADING) as LoadingUI;
            loadingUI.setProcessEvt(EventMacro.CHUNK_GENERATE_FINISH, 121);
            loadingUI.setFinishEvt(EventMacro.GENERATE_FIRST_WORLD_FINISH);
            EventManager.SendEvent(EventMacro.SELECT_WORLD, NetType.Single, GameConfig.Instance.defaultWorldConfigStr, "剧情模式" + timeStr, GameConfig.Instance.defalutSeed);
        }

        private void readAndSaveToFile(FileStream stream, string name)
        {

        }
    }
}
