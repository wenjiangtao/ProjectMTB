using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TestUI : MonoBehaviour {

	public static TestUI instance;
	Transform rankingList; 
	RankingList rankingListComp;
	public Text text;
	public UIOpBag itemBag;
	void Awake ()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {
		//rankingList = GameObject.Find ("/Canvas/RankingList").transform;
		//rankingListComp = rankingList.GetComponent<RankingList> ();
		Debug.Log ("Start~~~~~~~~");
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Update~~~~~~");
	}
	string[] textureList = {"cat1", "rabraduo", "dog1"};
	public void RandomUpdateItem(string id)
	{
		UIEventManager.SendEvent (UIEventManager.ET_UI_UPDATE, id, "Name", "text", "" + Random.Range (100, 99999));
		string iconName = textureList[Random.Range (0, 3)];
		UIEventManager.SendEvent (UIEventManager.ET_UI_UPDATE, id, "Icon", "", iconName);
		UIEventManager.SendEvent (UIEventManager.ET_UI_UPDATE, id, "HpBar", "", Random.Range (0f, 1f));
	}

	int idx = 1;
	void OnGUI () {
		return;
		if (GUI.Button(new Rect(0,0,50,25), "AddItem"))
		{
			rankingListComp.AddItem("" + idx);
			idx += 1;
		}
	}

	public void SetText()
	{
		text.text = "" + Random.Range (10000, 99999);
		Debug.Log ("OnClickButton~~~~~~~~");
	}

	public void CloseText()
	{
		if (itemBag == null)
			return;
		itemBag.Close ();
		//text.gameObject.SetActive (false);
	}

	public void OpenText ()
	{
		if (itemBag == null)
			itemBag = UIOperateBase.New<UIOpBag> ("Bag");
		else
			itemBag.Open ();
		//text.gameObject.SetActive (true);
	}
}
