using UnityEngine;
using System.Collections;

public class UIEventSender : MonoBehaviour {
	public string clickEventName;
	public string clickUpEventName;
	public string clickDownEventName;
	public string id;
	public void OnClick()
	{
		Debug.Log ("UIEventSender:" + clickEventName);
		EventManager.SendEvent (clickEventName, id);
	}

	public void OnClickDown()
	{
		EventManager.SendEvent (clickDownEventName, id);
	}

	public void OnClickUp()
	{
		EventManager.SendEvent (clickUpEventName, id);
	}

}
