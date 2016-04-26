using System;
using UnityEngine;
namespace MTB
{
	public class MTBPath
	{
		public Vector3 startPos;
		public Vector3 goalPos;
		
		public float runtime;
		
		public bool needsGround;
		
		public bool isReady
		{
			get { lock (this.stepsLock) { return this.foundPath && this.steps != null; } }
		}
		
		public bool foundPath
		{
			get { return this._foundPath; }
		}
		
		private bool _foundPath = true;
		
		public bool isValid
		{
			get { lock (this.stepsLock) { return this.steps.Length > 0; } }
		}
		
		public Vector3[] pathData
		{
			get { return this.steps; }
		}
		
		private Vector3[] steps;
		private object stepsLock = new object ();
		
		public MTBPath(Vector3 start, Vector3 goal)
		{
			this.startPos = start;
			this.goalPos = goal;
		}
		
		public void SetPathData(Vector3[] steps)
		{
			if (steps == null)
				this._foundPath = false;
			
			lock (this.stepsLock)
			{
				this.steps = steps;
			}
		}
	}
}

