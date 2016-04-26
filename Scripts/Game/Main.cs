using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MTB;
using LitJson;
public class Main : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<Watcher>();
		GameObject.Instantiate(Resources.Load("Prefabs/GameConfig") as GameObject);
		GameObject.Instantiate(Resources.Load("Prefabs/NetManager") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject);
        GameObject.Instantiate(Resources.Load("Prefabs/Game") as GameObject);
		#if UNITY_ANDROID
		Application.targetFrameRate = 60;
		#elif UNITY_EDITOR
		Application.targetFrameRate = -1;
		#endif
//		Application.targetFrameRate = -1;
		//因为ui初始化的时候用到了背包信息
		ItemManager.Instance.Init();
		BlockDataManager.Instance.Init();
		BackpackItemManager.Instance.Init();
        UIManager.Instance.showUI<StartUI>(UITypes.START);
//		Screen.SetResolution(1280,720,true);
		setDesignContentScale();
//		DataManagerM.Instance.Init();
		gameObject.AddComponent<GUITextDebug>();
//		string j = "{'a':'b','c':'d'}";
//		JsonData data = JsonMapper.ToObject(j);
//		Debug.Log(data["a"]);
//		Dictionary<string,string> d = JsonMapper.ToObject<Dictionary<string,string>>(j);

//		Dictionary<string,string> data = new Dictionary<string, string>();
//		data.Add("name","adfas");
//		data.Add("password","1111");
//		Action<UserMsg> callback = RegistCallback;
////		UserService.Instance.Regist(data,callback);
//		UserService.Instance.Login(data,callback);

    }
//	public void RegistCallback(UserMsg msg)
//	{
//		if(msg.id == UserMsg.SUCCESS)
//			Debug.Log("登录成功!");
//		else
//			Debug.Log("登录失败!");
//	}

	private int scaleWidth =0;
	private int scaleHeight =0;
	public void setDesignContentScale()
	{
		#if UNITY_ANDROID
		if(scaleWidth ==0 && scaleHeight ==0)
		{
			int width = Screen.currentResolution.width;
			int height = Screen.currentResolution.height;
			int designWidth = 1280;
			int designHeight = 720;
			float s1 = (float)designWidth / (float)designHeight;
			float s2 = (float)width / (float)height;
			if(s1 < s2) {
				designWidth = (int)Mathf.FloorToInt(designHeight * s2);
			} else if(s1 > s2) {
				designHeight = (int)Mathf.FloorToInt(designWidth / s2);
			}
			float contentScale = (float)designWidth/(float)width;
			if(contentScale < 1.0f) { 
				scaleWidth = designWidth;
				scaleHeight = designHeight;
			}
		}
		if(scaleWidth >0 && scaleHeight >0)
		{
			if(scaleWidth % 2 == 0) {
				scaleWidth += 1;
			} else {
				scaleWidth -= 1;					
			}
			Screen.SetResolution(scaleWidth,scaleHeight,true);
		}
		#endif
	}

	void OnApplicationPause(bool paused)
	{
		if (paused) {
		} else {
			setDesignContentScale();
		}
	}
}
