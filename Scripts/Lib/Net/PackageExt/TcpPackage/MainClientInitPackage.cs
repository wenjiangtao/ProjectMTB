using System;
using UnityEngine;
namespace MTB
{
	//当主机客户端连接到服务器时，需要初始化自己的信息到服务器上
	public class MainClientInitPackage : TcpPackage
	{
		public int roleId{get;set;}
		public Vector3 position{get;set;}
		public int aoId{get;set;}
		public int playerId{get;set;}
		public string worldConfigName{get;set;}
		public int seed{get;set;}
		public float time{get;set;}
		public MainClientInitPackage (int id)
			:base(id)
		{
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ClientPlayerInfo playerInfo = new ClientPlayerInfo();
			playerInfo.id = roleId;
			playerInfo.aoId = aoId;
			playerInfo.playerId = playerId;
			playerInfo.position =position;
			playerInfo.configName = worldConfigName;
			playerInfo.seed = seed;
			Debug.Log("初始化世界配置:" + worldConfigName);
			connectionWork.InitPlayer(playerInfo);
			NetManager.Instance.server.gameTime = time;
			NetManager.Instance.server.playerManager.AddPlayer(connectionWork.player,true);
			Debug.Log("服务器初始化主玩家信息完成！");
		}

		public override void WriteAllData ()
		{
			WriteInt(roleId);
			WriteVector3(position);
			WriteInt(aoId);
			WriteInt(playerId);
			WriteString(worldConfigName);
			WriteInt(seed);
			WriteFloat(time);
		}

		public override void ReadAllData ()
		{
			this.roleId = ReadInt();
			this.position = ReadVector3();
			this.aoId = ReadInt();
			this.playerId = ReadInt();
			worldConfigName = ReadString();
			this.seed = ReadInt();
			this.time = ReadFloat();
		}
	}
}

