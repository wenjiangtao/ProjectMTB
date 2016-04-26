using System;
using System.Collections.Generic;
using LitJson;
namespace MTB
{
	public class UserInfo
	{
		public static string IDKey = "id";
		public static string LoginNameKey = "name";
		public static string PasswordKey = "password";
		public static string NickNameKey = "nickName";
		public static string LastLoginTimeKey = "lastLoginTime";
		public static string IPKey = "lastLoginIP";
		public static string DeviceKey = "lastLoginDevice";
		public static string IdCardNoKey = "idCardNo";
		public static string EmailKey = "email";
		public static string MobileKey = "mobileNo";
		public static string QQKey = "qq";
		public static string WeChatKey = "weChat";
		public static string WeiBoKey = "weibo";
		public int ID{get;private set;}
		public string loginName{get;private set;}
		public string password{get;private set;}
		public string nickName{get;private set;}
		public string lastLoginTime{get;private set;}
		public string IP{get;private set;}
		public string device{get;private set;}
		public string idCardNo{get;private set;}
		public string email{get;private set;}
		public string mobile{get;private set;}
		public string qq{get;private set;}
		public string weChat{get;private set;}
		public string weibo{get;private set;}
		public UserInfo ()
		{
		}

		public void UpdateServerData(JsonData data)
		{
			if(data.Keys.Contains(IDKey))
			{
				ID = data[IDKey] == null ? 0 : (int)data[IDKey];
			}
			if(data.Keys.Contains(LoginNameKey))
			{
				loginName = data[LoginNameKey] == null ? "" : (string)data[LoginNameKey];
			}
			if(data.Keys.Contains(PasswordKey))
			{
				password = data[PasswordKey] == null ? "" : (string)data[PasswordKey];
			}
			if(data.Keys.Contains(NickNameKey))
			{
				nickName = data[NickNameKey] == null ? "" : (string)data[NickNameKey];
			}
			if(data.Keys.Contains(LastLoginTimeKey))
			{
				lastLoginTime = data[LastLoginTimeKey] == null ? "" : (string)data[LastLoginTimeKey];
			}
			if(data.Keys.Contains(IPKey))
			{
				IP = data[IPKey] == null ? "" : (string)data[IPKey];
			}
			if(data.Keys.Contains(DeviceKey))
			{
				device = data[DeviceKey] == null ? "" : (string)data[DeviceKey];
			}
			if(data.Keys.Contains(IdCardNoKey))
			{
				idCardNo = data[IdCardNoKey] == null ? "" :(string)data[IdCardNoKey];
			}
			if(data.Keys.Contains(EmailKey))
			{
				email = data[EmailKey] == null ? "" : (string)data[EmailKey];
			}
			if(data.Keys.Contains(MobileKey))
			{
				mobile = data[MobileKey] == null ? "" : (string)data[MobileKey];
			}
			if(data.Keys.Contains(QQKey))
			{
				qq = data[QQKey] == null ? "" : (string)data[QQKey];
			}
			if(data.Keys.Contains(WeChatKey))
			{
				weChat = data[WeChatKey] == null ? "" : (string)data[WeChatKey];
			}
			if(data.Keys.Contains(WeiBoKey))
			{
				weibo = data[WeiBoKey] == null ? "" : (string)data[WeiBoKey];
			}
		}
	}
}

