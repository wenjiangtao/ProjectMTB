using System;
using UnityEngine;
namespace MTB
{
	public class MovedWallOfAir : MonoBehaviour
	{
		private Transform _target;
		public void SetTarget(Transform target)
		{
			_target = target;
		}

		void Update()
		{
			if(_target != null)
			{
				transform.position = new Vector3(_target.position.x,transform.position.y,_target.position.z);
			}
		}

		public void Dispose()
		{
			_target = null;
		}
	}
}

