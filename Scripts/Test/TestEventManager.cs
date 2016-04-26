using UnityEngine;
using System.Collections;

public class TestEventManager : MonoBehaviour {

	void TestFunc(params object [] paras)
	{
		Debug.Log ("TestFunc:" + paras.Length);
	}
	// Use this for initialization
	void Start () {
		EventManager.RegisterEvent ("Test", TestFunc);
		EventManager.RegisterEvent ("Test", TestFunc);
		EventManager.SendEvent ("Test", 1, 2, 3);
		EventManager.UnRegisterEvent ("Test", TestFunc);
		EventManager.SendEvent ("Test", 1, 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
