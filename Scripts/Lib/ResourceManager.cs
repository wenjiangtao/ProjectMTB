using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager> {
	
	string editorResourcesDir = "Assets/Resources/";

	string[] GetAssetDirAndName(string path)
	{
		string assetName = "";
		string assetDir = "";
		int index = path.LastIndexOf ('/');
		if (index == -1) {
			assetName = path;
		} else {
			assetName = path.Substring(index + 1);
			assetDir = path.Substring(0,index + 1);
		}
		return new string[]{assetDir, assetName};
	}

	public Object LoadAsset<T> (string path) where T : Object
	{
		string [] pair = GetAssetDirAndName (path);

		return LoadAsset <T>(pair [0], pair [1]) ;
	}

	public Object LoadAsset<T> (string assetDir, string assetName) where T : Object
	{
//		#if UNITY_EDITOR
		string path = System.IO.Path.Combine(assetDir, assetName);
		//Debug.Log ("path:" + path);
		return Resources.Load(path, typeof(T));
//		#endif
//		return null;
	}

	public void LoadAssetAsync<T> (string path, System.Action<Object> action)
	{
		string[] pair = GetAssetDirAndName (path);
		LoadAssetAsync <T>(pair [0], pair [1], action); 
	}

	public void LoadAssetAsync<T> (string assetDir, string assetName, System.Action<Object> action)
	{
		Object res = null;
		#if UNITY_EDITOR
		res = Resources.Load(System.IO.Path.Combine(assetDir, assetName), typeof(T));
		action(res);
		return;
		#endif
	}	
}
