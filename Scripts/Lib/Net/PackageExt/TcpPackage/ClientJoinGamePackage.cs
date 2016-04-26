using System;
using UnityEngine;
namespace MTB
{
	//客户端收到服务器返回的进入游戏信息
	public class ClientJoinGamePackage : TcpPackage
	{
		public int playerId{get;set;}
		public Vector3 birthPlace{get;set;}
		public int aoId{get;set;}
		public string worldConfigName{get;set;}
		public int seed{get;set;}
		public float time{get;set;}
		public ClientJoinGamePackage (int id)
			:base(id)
		{
		}

		public override void ReadAllData ()
		{
			playerId = ReadInt();
			birthPlace = ReadVector3();
			aoId = ReadInt();
			worldConfigName = ReadString();
			seed = ReadInt();
			time = ReadFloat();
		}

		public override void WriteAllData ()
		{
			WriteInt(playerId);
			WriteVector3(birthPlace);
			WriteInt(aoId);
			WriteString(worldConfigName);
			WriteInt(seed);
			WriteFloat(time);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			PlayerInfo info = new PlayerInfo();
			info.playerId = playerId;
			info.aoId = aoId;
			info.position = birthPlace;
			info.isMe = true;
			Debug.Log("收到世界配置为:" + worldConfigName);
			EventManager.SendEvent(NetEventMacro.ON_JION_GAME,info,worldConfigName,seed,time);
		}
	}
}

