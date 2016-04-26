using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class LoginUI : UIOperateBase
	{
		private InputField _accountInput;
		private InputField _passwordInput;
		private Button _loginBtn;
		private Button _registerBtn;
		private Button _forgetBtn;
		private Button _closeBtn;
		public override void Init(params object[] paras)
		{
			uiType = UITypes.LOGIN;
			base.Init(paras);
		}
		
		public override void InitView()
		{
			base.InitView();
			_accountInput = this.transform.Find("Number/AccountInput").GetComponent<InputField>();
			_passwordInput = this.transform.Find("Password/PasswordInput").GetComponent<InputField>();
			_loginBtn = this.transform.Find("LoginBtn").GetComponent<Button>();
			_registerBtn = this.transform.Find("RegisterBtn").GetComponent<Button>();
			_forgetBtn = this.transform.Find("ForgetBtn").GetComponent<Button>();
			_closeBtn = this.transform.Find("CloseBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_loginBtn.onClick.AddListener(delegate() {
				Action<UserMsg> action = (UserMsg msg)=>{
					if(msg.id == UserMsg.SUCCESS)
					{
						UIManager.Instance.showUI<StartGameUI>(UITypes.START_GAME);
						UIManager.Instance.closeUI(uiType);
					}
					else
					{
						UnityEngine.Debug.Log(msg.msg);
					}
				};
				Login(action);
			});

			_registerBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<RegisterMainUI>(UITypes.REGISTER);
				UIManager.Instance.closeUI(uiType);
			});

			_forgetBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<PasswordFoundUI>(UITypes.PASSWORDFOUND);
				UIManager.Instance.closeUI(uiType);
			});

			_closeBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<StartGameUI>(UITypes.START_GAME);
				UIManager.Instance.closeUI(uiType);
			});
		}

		private void ClearInput()
		{
			_accountInput.text = "";
			_passwordInput.text = "";
		}

		public override void Close ()
		{
			ClearInput();
			base.Close ();
		}

		private void Login(Action<UserMsg> action)
		{
			string account = _accountInput.text;
			if(string.IsNullOrEmpty(account))
			{
				Fail(action,"账号不能为空!");
				return;
			}
			string password = _passwordInput.text;
			if(string.IsNullOrEmpty(password))
			{
				Fail(action,"密码不能为空!");
				return;
			}
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add(UserInfo.LoginNameKey,account);
			data.Add(UserInfo.PasswordKey,password);
			UserService.Instance.Login(data,action);
		}

		private void Fail(Action<UserMsg> action,string msg)
		{
			UserMsg userMsg = new UserMsg(UserMsg.FAIL,msg);
			action.Invoke(userMsg);
		}

		protected override void removeEvents ()
		{
			base.removeEvents ();
			_loginBtn.onClick.RemoveAllListeners();
			_registerBtn.onClick.RemoveAllListeners();
			_forgetBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.RemoveAllListeners();
		}
	}
}

