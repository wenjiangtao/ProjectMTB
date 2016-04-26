using System;
using UnityEngine;
namespace MTB
{
	public class MonsterInfo : GameObjectInfo
	{
		public int monsterId{get{return objectId;}set{objectId = value;}}
		public MonsterInfo ()
		{
			monsterId = 1;
			groupId = 1;
			aoId = 0;
			position = Vector3.zero; 
			isNetObj = false;
		}
	}
}

