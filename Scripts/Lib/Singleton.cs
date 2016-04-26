using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour {
	static T instance = null;
	static GameObject go;
	public static T Instance {
		set {
			instance = value;
		}
		get {
			if (instance != null)
				return instance;
			if (go == null)
				go = new GameObject(typeof(T).Name);
			instance = go.AddComponent<T>();
			return instance;
		}
	}
}
