using UnityEngine;
using System.Collections;

public class Watcher : MonoBehaviour {


	public static Watcher instance;
	public int worldDictTimes = 0;
	public int initChunkTimes = 0;
	public bool isBreak = false;
	void Awake ()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
