using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class MTBGesture
	{
		//The index of the finger that fired the event, or -1 for a two fingers gesture.
		public int fingerIndex;
		//The number of active touches
		public int touchCount;
		//The start position of the current gesture that fired the event.
		public Vector2 startPosition;
		//The current position of the current gesture that fired the event.
		public Vector2 position;
		//The delta position since last call.
		public Vector2 deltaPosition;
		//The elapsed time in second since the begin of gesture.
		public float actionTime;
		//The delta time since last call.
		public float deltaTime;		
	}
}

