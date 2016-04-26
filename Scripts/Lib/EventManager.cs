using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager {
	static Dictionary<string, List<DelegateDef.ParamsDelegate>> eventHandlerMap;
	public static Dictionary<string, List<DelegateDef.ParamsDelegate>> EventHandlerMap{
		get{
			if (eventHandlerMap == null)
				eventHandlerMap = new Dictionary<string, List<DelegateDef.ParamsDelegate>>();
			return eventHandlerMap;
		}
	}

	public static void RegisterEvent(string name, DelegateDef.ParamsDelegate handler)
	{
		List<DelegateDef.ParamsDelegate> handlerList;
		if (EventHandlerMap.ContainsKey(name))
		{
			handlerList = EventHandlerMap[name];
		}
		else{
			handlerList = EventHandlerMap[name] = new List<DelegateDef.ParamsDelegate>();
		}
		if (handlerList.Contains (handler))
			return;
		handlerList.Add (handler);
	}

	public static void UnRegisterEvent(string name, DelegateDef.ParamsDelegate handler)
	{
		if (EventHandlerMap.ContainsKey(name))
		{
			List<DelegateDef.ParamsDelegate>  handlerList = EventHandlerMap[name];
			handlerList.Remove(handler);
		}
	}

	public static void SendEvent(string name, params object [] paras)
	{

		if (EventHandlerMap.ContainsKey(name))
		{
			List<DelegateDef.ParamsDelegate>  handlerList = new List<DelegateDef.ParamsDelegate>(EventHandlerMap[name]);
			foreach (DelegateDef.ParamsDelegate handler in handlerList)
			{
				handler(paras);
			}
		}
	}
}
