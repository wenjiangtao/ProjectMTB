using System;
using System.Collections.Generic;
using UnityEngine.UI;
namespace MTB
{
	public class RegisterUI : UISubPanel
	{
		private Button _confirmBtn;
		private InputField _accountInput;
		private InputField _passwordInput;
		private InputField _confirmPWInput;
		private InputField _phoneInput;
		private InputField _emailInput;
		private Button _closeBtn;
		public override void InitView ()
		{
			base.InitView ();
			_confirmBtn = this.transform.Find("ConfirmBtn").GetComponent<Button>();
			_accountInput = this.transform.Find("Account/AccountInput").GetComponent<InputField>();
			_passwordInput = this.transform.Find("Password/PasswordInput").GetComponent<InputField>();
			_confirmPWInput = this.transform.Find("ConfirmPassword/ConfirmPWInput").GetComponent<InputField>();
			_phoneInput = this.transform.Find("Phone/PhoneInput").GetComponent<InputField>();
			_emailInput = this.transform.Find("Email/EmailInput").GetComponent<InputField>();
			_closeBtn = this.transform.Find("CloseBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_confirmBtn.onClick.AddListener(delegate() {
				Action<UserMsg> action = (UserMsg msg)=>{
					if(msg.id == UserMsg.SUCCESS)
					{
						mainPanel.CloseSubPanel(this.panelId);
						mainPanel.OpenSubPanel(2);
					}
					else
					{
						UnityEngine.Debug.Log(msg.msg);
					}
				};
				Register(action);
			});

			_closeBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				UIManager.Instance.showUI<LoginUI>(UITypes.LOGIN);
				UIManager.Instance.closeUI(mainPanel.uiType);
			});
		}

		private void Register(Action<UserMsg> action)
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
			string confirmPW = _confirmPWInput.text;
			if(string.IsNullOrEmpty(confirmPW))
			{
				Fail(action,"二次密码不能为空!");
				return;
			}
			if(!string.Equals(password,confirmPW))
			{
				Fail(action,"二次密码错误!");
				return;
			}
			string phone = _phoneInput.text;
			string email = _emailInput.text;
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add(UserInfo.LoginNameKey,account);
			data.Add(UserInfo.PasswordKey,password);
			data.Add(UserInfo.MobileKey,phone);
			data.Add(UserInfo.EmailKey,email);
			Action<UserMsg> registAction = (UserMsg resultMsg) => {
				if(resultMsg.id == UserMsg.FAIL)
				{
					Fail(action,resultMsg.msg);
				}
				else
				{
					Login(action);
				}
			};
			UserService.Instance.Regist(data,registAction);
		}

		private void Login(Action<UserMsg> action)
		{
			string account = _accountInput.text;
			string password = _passwordInput.text;
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
			_confirmBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.RemoveAllListeners();
		}
	}
}

