using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	//玩家进入游戏后请求附近其他玩家的消息
	public class LoadScenePlayerPackage : TcpPackage
	{
		private List<PlayerInfo> list;
		public LoadScenePlayerPackage (int id)
			:base(id)
		{
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			if(list == null)return;
			for (int i = 0; i < list.Count; i++) {
				list[i].isNetObj = true;
				HasActionObjectManager.Instance.playerManager.InitPlayer(list[i]);
			}
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			List<int> viewPlayers = connectionWork.player.viewPlayers;
			list = new List<PlayerInfo>(viewPlayers.Count);
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer player = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				PlayerInfo info = new PlayerInfo();
				info.aoId = player.aoId;
				info.playerId = player.playerId;
				info.position = player.position;
				info.isMe = false;
				list.Add(info);
			}
			connectionWork.SendPackage(this);
		}

		public override void WriteAllData ()
		{
			if(list != null)
			{
				WriteInt(list.Count);
				for (int i = 0; i < list.Count; i++) {
					WritePlayerInfo(list[i]);
				}
			}
			else
				WriteInt(0);
		}

		public override void ReadAllData ()
		{
			int count = ReadInt();
			list = new List<PlayerInfo>(count);
			for (int i = 0; i < count; i++) {
				list.Add(ReadPlayerInfo());
			}
		}

		private PlayerInfo ReadPlayerInfo()
		{
			PlayerInfo info = new PlayerInfo();
			info.aoId = ReadInt();
			info.playerId = ReadInt();
			info.position = ReadVector3();
			info.isMe = ReadBool();
			return info;
		}

		private void WritePlayerInfo(PlayerInfo info)
		{
			WriteInt(info.aoId);
			WriteInt(info.playerId);
			WriteVector3(info.position);
			WriteBool(info.isMe);
		}
	}
}

