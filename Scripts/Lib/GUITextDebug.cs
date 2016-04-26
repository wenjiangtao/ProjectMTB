using System;
using UnityEngine;
namespace MTB
{
	public class GUITextDebug : MonoBehaviour
	{
		private int positionCheck = 2;  
		private static string windowText = "";  
		private Vector2 scrollViewVector = Vector2.zero;  
		private GUIStyle debugBoxStyle;  
		
		private int w;
		private int h;
		public static void debug(string newString)  
		{  
			windowText = newString + "\n" + windowText;  
//			UnityEngine.Debug.Log(newString);  
		}  
		
		void Start()   
		{  
			w = Screen.width;
			h = Screen.height;
			debugBoxStyle = new GUIStyle();  
			debugBoxStyle.alignment = TextAnchor.UpperLeft;  
			debugBoxStyle.fontSize = h / 50;
			debugBoxStyle.normal.textColor = Color.blue;
		}  
		
		
		void OnGUI()   
		{  
			if(positionCheck == 1)
			{
			GUI.depth = 0;    
			GUI.BeginGroup(new Rect(0, 50, 600, 200));  
			
			scrollViewVector = GUI.BeginScrollView(new Rect (0, 0.0f, 600f, 200f),   
			                                       scrollViewVector,   
			                                       new Rect (0.0f, 0.0f, 4000.0f, 2000.0f));  
			GUI.Box(new Rect(0, 0.0f, 4000f, 2000.0f), windowText, debugBoxStyle);  
			GUI.EndScrollView();  
			
			GUI.EndGroup (); 

			}
			if (GUI.Button(new Rect(0, 0,100,50), "调试"))  
			{  
				if (positionCheck == 1)  
				{  
					positionCheck = 2;  
				}  
				else   
				{  
					positionCheck = 1;  
				}  
			}  
			
			if (GUI.Button(new Rect(100,0.0f,100,50),"清除"))  
			{  
				windowText = "";  
			}  
		}  
	}
}

