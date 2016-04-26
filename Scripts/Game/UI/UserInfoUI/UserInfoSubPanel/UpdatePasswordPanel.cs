using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class UpdatePasswordPanel : UISubPanel
	{
		private Button _confirmBtn;
		private Button _cancelBtn;
		private InputField _passwordInput;
		private InputField _newPWInput;
		private InputField _replayPWInput;

		public override void InitView ()
		{
			base.InitView ();
			_confirmBtn = this.transform.Find("ConfirmBtn").GetComponent<Button>();
			_cancelBtn = this.transform.Find("CancelBtn").GetComponent<Button>();
			_passwordInput = this.transform.Find("Password/PasswordInput").GetComponent<InputField>();
			_newPWInput = this.transform.Find("NewPW/NewPWInput").GetComponent<InputField>();
			_replayPWInput = this.transform.Find("ReplayPW/ReplayPWInput").GetComponent<InputField>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_confirmBtn.onClick.AddListener(delegate() {
				Action<UserMsg> action = (UserMsg msg)=>{
					if(msg.id == UserMsg.SUCCESS)
					{
						UnityEngine.Debug.Log("密码更新成功!");
						mainPanel.Refresh();
						mainPanel.CloseSubPanel(this.panelId);
					}
					else
					{
						UnityEngine.Debug.Log(msg.msg);
					}
				};
				UpdatePassword(action);
			});
			_cancelBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
			});
		}

		private void UpdatePassword(Action<UserMsg> action)
		{
			string password = _passwordInput.text;
			if(string.IsNullOrEmpty(password))
			{
				Fail(action,"密码不能为空!");
				return;
			}
			string newPW = _newPWInput.text;
			if(string.IsNullOrEmpty(newPW))
			{
				Fail(action,"新密码不能为空!");
				return;
			}
			string replayPW = _replayPWInput.text;
			if(string.IsNullOrEmpty(replayPW))
			{
				Fail(action,"重复密码不能为空!");
				return;
			}
			if(!string.Equals(newPW,replayPW))
			{
				Fail(action,"重复密码不正确!");
				return;
			}
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add("oldPassword",password);
			data.Add("newPassword",newPW);
			UserService.Instance.UpdateUser(data,action,UserUpdateOperateType.Password);
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
			_cancelBtn.onClick.RemoveAllListeners();
		}
	}
}

