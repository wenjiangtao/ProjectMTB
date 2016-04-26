using UnityEngine;
using System.Collections;

public class UtilUI{
	public static GameObject Create(GameObject prefab, GameObject parent)
	{
		GameObject go = GameObject.Instantiate (prefab);
		RectTransform rectTrans = go.GetComponent<RectTransform> ();
		if (parent != null)
			rectTrans.SetParent (parent.transform, false);
		return go;
	}
}
