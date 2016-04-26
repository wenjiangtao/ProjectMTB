using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIEventManager {

	public const string ET_UI_UPDATE = "UIUpdate";
	public const string ET_UI_CLICK = "UIClick";
	public delegate void EventHandler(params object[] paras);
	static Dictionary<string, Dictionary<string, List<EventHandler> > > eventHandlerMap = new Dictionary<string, Dictionary<string, List<EventHandler>>> ();
	
	public static void RegisterEvent(string eventType, string id, EventHandler handler)
	{
		if (!eventHandlerMap.ContainsKey (eventType)) {
			eventHandlerMap[eventType] = new Dictionary<string, List<EventHandler>>();
		}
		Dictionary<string, List<EventHandler>> subEventHandlerMap = eventHandlerMap [eventType];
		if (!subEventHandlerMap.ContainsKey (id)) {
			subEventHandlerMap[id] = new List<EventHandler>();
		}
		List<EventHandler> handlerList = subEventHandlerMap [id];
		if (!handlerList.Contains (handler))
			handlerList.Add (handler);
	}

	public static void UnRegisterEvent(string eventType, string id, EventHandler handler)
	{
		if (!eventHandlerMap.ContainsKey(eventType))
			return;
		Dictionary<string, List<EventHandler>> subEventHandlerMap = eventHandlerMap [eventType];
		if (!subEventHandlerMap.ContainsKey (id))
			return;
		List<EventHandler> handlerList = subEventHandlerMap [id];
		handlerList.Remove (handler);
	}

	public static void SendEvent(string eventType, string id, params object [] paras)
	{
		if (!eventHandlerMap.ContainsKey(eventType))
			return;
		Dictionary<string, List<EventHandler>> subEventHandlerMap = eventHandlerMap [eventType];
		if (!subEventHandlerMap.ContainsKey (id))
			return;
		List<EventHandler> handlerList = subEventHandlerMap [id];
		foreach (EventHandler handler in handlerList) {
			handler(paras);
		}
	}
}
