using UnityEngine;
using System.Collections;

public class GameUIRoot{
	static GameObject rootGo;
	static RectTransform rootRectTrans;
	public static GameObject RootGo
	{
		get {
			if (rootGo == null)
			{
				rootGo = GameObject.Find("/UI");
				if (rootGo == null)
				{
					GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/UI") as GameObject);
					go.name = "UI";
					rootGo = go;
				}
					
				rootGo = GameObject.Find("/UI");
				rootRectTrans = rootGo.GetComponent<RectTransform>();
			}
			return rootGo;
		}
	}

	public RectTransform RootRectTransform
	{
		get {
			return rootRectTrans;
		}
	}
}
