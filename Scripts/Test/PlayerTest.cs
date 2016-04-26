using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
public class PlayerTest : MonoBehaviour
{
	public float moveSpeed = 0.5f;
	public void Update()
	{
		transform.Translate(new Vector3(Input.GetAxis("Horizontal") * moveSpeed,0,Input.GetAxis("Vertical") * moveSpeed));
	}

	public void OnGUI()
	{
//		if(GUI.Button(new Rect(0,0,100,50),"移除块"))
//		{
//		}
	}

}

}