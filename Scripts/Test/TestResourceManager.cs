using UnityEngine;
using System.Collections;

public class TestResourceManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Instantiate(ResourceManager.Instance.LoadAsset<GameObject> ("Other/Cube"));
	}

	static TestResourceManager()
	{
		Game.instance.gameObject.AddComponent<TestResourceManager>();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
