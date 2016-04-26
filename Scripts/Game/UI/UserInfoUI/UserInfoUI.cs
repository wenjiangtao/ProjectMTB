using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class UserInfoUI : UIMainPanel
	{
		private Button _returnBtn;
		private Button _userInfoBtn;
		private Button _RealNameBtn;
		private Button _MoneyBtn;
		private int curPanelId;
		public override void Init (params object[] paras)
		{
			uiType = UITypes.USERINFO;
			base.Init (paras);
		}
		protected override void InitSubPanel ()
		{
			RegisterSubPanel<UserInfoPanel>(1,"UserInfoPanel");
			RegisterSubPanel<RealNamePanel>(2,"RealNamePanel");
			RegisterSubPanel<MoneyPanel>(3,"MoneyPanel");
		}


		public override void OpenSubPanel (int id)
		{
			base.OpenSubPanel (id);
			curPanelId = id;
		}

		public override void InitView ()
		{
			base.InitView ();
			_returnBtn = this.transform.Find("ReturnBtn").GetComponent<Button>();
			_userInfoBtn = this.transform.Find("UserInfoTab/BaseInfoBtn/Button").GetComponent<Button>();
			_RealNameBtn = this.transform.Find("UserInfoTab/RealNameBtn/Button").GetComponent<Button>();
			_MoneyBtn = this.transform.Find("UserInfoTab/MoneyBtn/Button").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_returnBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<StartGameUI>(UITypes.START_GAME);
				UIManager.Instance.closeUI(uiType);
			});
			_userInfoBtn.onClick.AddListener(delegate() {
				if(curPanelId != 1)
				{
					CloseSubPanel(curPanelId);
					OpenSubPanel(1);
				}
			});
			_RealNameBtn.onClick.AddListener(delegate() {
				if(curPanelId != 2)
				{
					CloseSubPanel(curPanelId);
					OpenSubPanel(2);
				}
			});
			_MoneyBtn.onClick.AddListener(delegate() {
				if(curPanelId != 3)
				{
					CloseSubPanel(curPanelId);
					OpenSubPanel(3);
				}
			});
		}


		protected override void removeEvents ()
		{
			base.removeEvents ();
			_returnBtn.onClick.RemoveAllListeners();
			_userInfoBtn.onClick.RemoveAllListeners();
			_RealNameBtn.onClick.RemoveAllListeners();
			_MoneyBtn.onClick.RemoveAllListeners();
		}
		public override void Open ()
		{
			base.Open ();
			OpenSubPanel(1);
		}
	}
}

