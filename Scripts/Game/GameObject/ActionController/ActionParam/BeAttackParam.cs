using System;
using UnityEngine;
namespace MTB
{
	public class BeAttackParam : ActionParam
	{
		public readonly Vector3 attackDirection;
		public BeAttackParam (Vector3 attackDirection)
		{
			this.attackDirection = attackDirection;
		}
	}
}

