using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {

	public void Do()
	{
		GameObject.Destroy (gameObject);
	}
}
