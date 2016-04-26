using UnityEngine;
using System.Collections;

public class UIItemRankingList : UIItemBase {


	public override void InitElements()
	{
		Transform nameTrans = mTrans.FindChild ("Name");
		Transform iconTrans = mTrans.FindChild ("Icon");
		Transform hpBarTrans = mTrans.FindChild ("HpBar");
		AddElement ("Name", nameTrans.GetComponent<UGUIText> ());
		AddElement ("Icon", iconTrans.GetComponent<UGUIImage> ());
		AddElement ("HpBar", hpBarTrans.GetComponent<UGUIProgressBar> ());
	}

	public void OnDestroy()
	{

	}

	public void OnClick()
	{
		Debug.Log ("OnClick:" + this.id);
		TestUI.instance.RandomUpdateItem (this.id);
	}
}
