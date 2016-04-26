using UnityEngine;
using System.Collections;

public class RankingList : MonoBehaviour {
	int idx = 0;
	public void AddItem(string id)
	{
		GameObject prefab = Resources.Load ("UI/Prefabs/RankingListItem") as GameObject;
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		//go.transform.parent = gameObject.transform;
		RectTransform rectTrans = go.GetComponent<RectTransform>();


		Vector3 localPosition = rectTrans.anchoredPosition3D;
		Debug.Log ("localPosition:" + localPosition);
		localPosition.y = -idx * 60;
		idx += 1;
		rectTrans.SetParent (gameObject.transform);
		rectTrans.localScale = Vector3.one;
		rectTrans.anchoredPosition3D = localPosition;
		UIItemRankingList rankingListItem = go.GetComponent<UIItemRankingList> ();
		rankingListItem.Init (id);
		//Vector3 position = rectTrans.localPosition;
		//position.y = idx * 60;
		//rectTrans.localPosition = position;
		//rectTrans.rect
	}


}
