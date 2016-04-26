using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class HintMsg : Singleton<HintMsg>{
	class Msg
	{
		public string text;
	}

	public void Hint(string msg)
	{
		Msg hintMsg = new Msg();
		hintMsg.text = msg;
		msgQueue.Enqueue (hintMsg);
	}

	Queue<Msg> msgQueue = new Queue<Msg>();
	float lastMsgShowTime = 0f;
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 5 != 0)
			return;
		if (msgQueue.Count == 0)
			return;
		if (Time.realtimeSinceStartup - lastMsgShowTime >= 0.5f) {
			Msg msg = msgQueue.Dequeue();
			GameObject go = GameObject.Instantiate(Resources.Load("Test/HintText") as GameObject) as GameObject;
			Text text = go.GetComponent<Text>();
			text.text = msg.text;
			RectTransform rectTrans = go.GetComponent<RectTransform>();
			rectTrans.SetParent(TestUI.instance.transform);
			lastMsgShowTime = Time.realtimeSinceStartup;
		}
	}
}
