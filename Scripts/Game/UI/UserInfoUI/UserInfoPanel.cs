using System;
using UnityEngine.UI;
namespace MTB
{
	public class UserInfoPanel : UIMainPanel
	{
		private InputField _nameInput;
		private Button _updatePasswordBtn;
		private Button _loginInfoSearchBtn;
		private Button _emailCheckBtn;
		private Button _emailUpdateBtn;
		private Button _phoneCheckBtn;
		private Button _phoneUpdateBtn;

		private Text _idTxt;
		private Text _loginNameTxt;
		private Text _loginPWTxt;
		private Text _lastLoginTimeTxt;
		private Text _lastLoginIpTxt;
		private Text _lastLoginDeviceTxt;
		private Text _emailTxt;
		private Text _phoneTxt;
		private Text _QQTxt;
		private Text _weiXinTxt;
		private Text _WeiBoTxt;

		protected override void InitSubPanel ()
		{
			RegisterSubPanel<UpdatePasswordPanel>(1,"UpdatePasswordPanel");
			RegisterSubPanel<UpdatePhonePanel>(2,"UpdatePhonePanel");
			RegisterSubPanel<UpdateEmailPanel>(3,"UpdateEmailPanel");
			RegisterSubPanel<CheckPhonePanel>(4,"CheckPhonePanel");
			RegisterSubPanel<CheckEmailPanel>(5,"CheckEmailPanel");
		}


		public override void InitView ()
		{
			base.InitView();
			_nameInput = this.transform.Find("Content/Gridview/NameLabel/NameInput").GetComponent<InputField>();
			_updatePasswordBtn = this.transform.Find("Content/Gridview/LoginPasswordLabel/UpdatePasswordBtn").GetComponent<Button>();
			_loginInfoSearchBtn = this.transform.Find("Content/Gridview/LastLoginIpLabel/LoginInfoSearchBtn").GetComponent<Button>();
			_emailCheckBtn = this.transform.Find("Content/Gridview/EmailLabel/EmailCheckBtn").GetComponent<Button>();
			_emailUpdateBtn = this.transform.Find("Content/Gridview/EmailLabel/EmailUpdateBtn").GetComponent<Button>();
			_phoneCheckBtn = this.transform.Find("Content/Gridview/PhoneLabel/PhoneCheckBtn").GetComponent<Button>();
			_phoneUpdateBtn = this.transform.Find("Content/Gridview/PhoneLabel/PhoneUpdateBtn").GetComponent<Button>();
		
			_idTxt = this.transform.Find("Content/Gridview/IDLabel/IDTxt").GetComponent<Text>();
			_loginNameTxt = this.transform.Find("Content/Gridview/LoginNameLabel/LoginNameTxt").GetComponent<Text>();
			_loginPWTxt = this.transform.Find("Content/Gridview/LoginPasswordLabel/LoginPasswordTxt").GetComponent<Text>();
			_lastLoginTimeTxt = this.transform.Find("Content/Gridview/LastLoginTimeLabel/LastLoginTimeTxt").GetComponent<Text>();
			_lastLoginIpTxt = this.transform.Find("Content/Gridview/LastLoginIpLabel/LastLoginIpTxt").GetComponent<Text>();
			_lastLoginDeviceTxt = this.transform.Find("Content/Gridview/LastLoginDeviceLabel/LastLoginDeviceTxt").GetComponent<Text>();
			_emailTxt = this.transform.Find("Content/Gridview/EmailLabel/EmailTxt").GetComponent<Text>();
			_phoneTxt = this.transform.Find("Content/Gridview/PhoneLabel/PhoneTxt").GetComponent<Text>();
			_QQTxt = this.transform.Find("Content/Gridview/QQLabel/QQTxt").GetComponent<Text>();
			_weiXinTxt = this.transform.Find("Content/Gridview/WeiXinLabel/WeiXinTxt").GetComponent<Text>();
			_WeiBoTxt = this.transform.Find("Content/Gridview/WeiBoLabel/WeiBoTxt").GetComponent<Text>();
		}

		public override void Refresh ()
		{
			UserInfo userInfo = UserService.Instance.userInfo;
			_idTxt.text = userInfo.ID.ToString();
			_nameInput.text = userInfo.nickName;
			_loginNameTxt.text = userInfo.loginName;
			_loginPWTxt.text = userInfo.password;
			_lastLoginTimeTxt.text = userInfo.lastLoginTime;
			_lastLoginIpTxt.text = userInfo.IP;
			_lastLoginDeviceTxt.text = userInfo.device;
			_emailTxt.text = userInfo.email;
			_phoneTxt.text = userInfo.mobile;
			_QQTxt.text = userInfo.qq;
			_weiXinTxt.text = userInfo.weChat;
			_WeiBoTxt.text = userInfo.weibo;
		}

		public override void Open ()
		{
			base.Open ();
			Refresh();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_updatePasswordBtn.onClick.AddListener(delegate() {
				OpenSubPanel(1);
			});
			_loginInfoSearchBtn.onClick.AddListener(delegate() {
			});
			_emailCheckBtn.onClick.AddListener(delegate() {
				OpenSubPanel(2);
			});
			_emailUpdateBtn.onClick.AddListener(delegate() {
				OpenSubPanel(3);
			});
			_phoneCheckBtn.onClick.AddListener(delegate() {
				OpenSubPanel(4);
			});
			_phoneUpdateBtn.onClick.AddListener(delegate() {
				OpenSubPanel(5);
			});
		}

		protected override void removeEvents ()
		{
			base.removeEvents ();
			_updatePasswordBtn.onClick.RemoveAllListeners();
			_loginInfoSearchBtn.onClick.RemoveAllListeners();
			_emailCheckBtn.onClick.RemoveAllListeners();
			_emailUpdateBtn.onClick.RemoveAllListeners();
			_phoneCheckBtn.onClick.RemoveAllListeners();
			_phoneUpdateBtn.onClick.RemoveAllListeners();
		}
	}
}

