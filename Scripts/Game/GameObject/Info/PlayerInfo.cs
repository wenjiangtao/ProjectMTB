using System;
using UnityEngine;
namespace MTB
{
	public class PlayerInfo : GameObjectInfo
	{
		public int playerId{
			get{return objectId;}
			set{objectId = value;}
		}
		public bool isMe{get;set;}
		public PlayerInfo ()
		{
			playerId = 1;
			groupId = 1;
			aoId = 0;
			isMe = false;
			position = Vector3.zero; 
			isNetObj = false;
		}
	}
}

