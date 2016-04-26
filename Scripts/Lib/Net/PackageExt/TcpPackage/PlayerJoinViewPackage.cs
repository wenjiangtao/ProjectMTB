using System;
using UnityEngine;
namespace MTB
{
	//玩家进入视野范围
	public class PlayerJoinViewPackage : TcpPackage
	{
		public int playerId{get;set;}
		public Vector3 birthPlace{get;set;}
		public int aoId{get;set;}
		public PlayerJoinViewPackage (int id)
			:base(id)
		{
		}
		public override void ReadAllData ()
		{
			playerId = ReadInt();
			birthPlace = ReadVector3();
			aoId = ReadInt();
		}
		
		public override void WriteAllData ()
		{
			WriteInt(playerId);
			WriteVector3(birthPlace);
			WriteInt(aoId);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			PlayerInfo info = new PlayerInfo();
			info.playerId = playerId;
			info.aoId = aoId;
			info.position = birthPlace;
			info.isMe = false;
			info.isNetObj = true;
			UnityEngine.Debug.Log("玩家roleId:" + aoId+"进入视野!");
			HasActionObjectManager.Instance.playerManager.InitPlayer(info);
		}
	}
}

