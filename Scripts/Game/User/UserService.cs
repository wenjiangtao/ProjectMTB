using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
namespace MTB
{
	public class UserService : Singleton<UserService>
	{
		public UserInfo userInfo{get;private set;}
		public bool isLogin{get;private set;}
		private Dictionary<UserUpdateOperateType,string> map;

		void Start()
		{
			isLogin = false;
			map = new Dictionary<UserUpdateOperateType, string>();
			map.Add(UserUpdateOperateType.Password,"modifyPassword.do");
			map.Add(UserUpdateOperateType.Email,"");
			map.Add(UserUpdateOperateType.Mobile,"");
		}

		public void Regist(Dictionary<string,string> data,Action<UserMsg> callback)
		{
			StartCoroutine(UserOperate("http://10.18.15.53:8080/mtb-web/user/register.do",data,callback));
		}

		public void Login(Dictionary<string,string> data,Action<UserMsg> callback)
		{
			if(!isLogin)
			{
				Action<UserMsg> callbackAction = (UserMsg msg) =>{
					if(msg.id == UserMsg.SUCCESS)
					{
						isLogin = true;
						userInfo = new UserInfo();
						userInfo.UpdateServerData(msg.data);
					}
					callback.Invoke(msg);
				};
				StartCoroutine(UserOperate("http://10.18.15.53:8080/mtb-web/user/login.do",data,callbackAction));
			}
			else
			{
				UserMsg msg = new UserMsg(UserMsg.FAIL,"用户已登录!");
				callback.Invoke(msg);
			}
		}

		public void UpdateUser(Dictionary<string,string> data,Action<UserMsg> callback,UserUpdateOperateType type)
		{
			string s;
			map.TryGetValue(type,out s);
			if(!string.IsNullOrEmpty(s))
			{
				data.Add(UserInfo.IDKey,userInfo.ID.ToString());
				string url = "http://10.18.15.53:8080/mtb-web/user/" + map[type];
				StartCoroutine(UserOperate(url,data,callback));
			}
			else
			{
				Debug.LogError("不存在当前的用户数据更新类型:" + type);
			}
		}

		private IEnumerator UserOperate(string url,Dictionary<string,string> data,Action<UserMsg> callback)
		{
			WWWForm form = new WWWForm();
			foreach (var item in data) {
				form.AddField(item.Key,item.Value);
			}
			WWW www = new WWW(url,form);
			yield return www;
			UserMsg msg;
			if(www.error != null)
			{
				msg = new UserMsg(UserMsg.FAIL,www.error);
			}
			else
			{
				JsonData jsonData = JsonMapper.ToObject(www.text);
				int id = (int)jsonData["result"];
				if(jsonData.Keys.Contains("data"))
				{
					msg = new UserMsg(id,jsonData["data"]);
				}
				else
				{
					msg = new UserMsg(id);
				}
			}
			callback.Invoke(msg);
			yield return null;
		}

		public void UnLogin()
		{
			if(isLogin)
			{
				isLogin = false;
			}
		}
	}

	public enum UserUpdateOperateType
	{
		Password,
		Email,
		Mobile
	}
}

