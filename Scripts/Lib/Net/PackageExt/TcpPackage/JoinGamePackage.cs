using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	//客户端向服务器发送进入游戏消息
	public class JoinGamePackage : TcpPackage
	{
		public int roleId{get;set;}
		public int playerId{get;set;}
		public JoinGamePackage (int id)
			:base(id)
		{
		}

		public override void ReadAllData ()
		{
			roleId = ReadInt();
			playerId = ReadInt();
		}

		public override void WriteAllData ()
		{
			WriteInt(roleId);
			WriteInt(playerId);
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			//在服务器中初始化当前加入玩家的信息
			int aoId = AoIdManager.instance.getAoId();
			Vector3 birthPlace = NetManager.Instance.server.playerManager.mainPlayer.position;
			ClientPlayerInfo playerInfo = new ClientPlayerInfo();
			playerInfo.id = roleId;
			playerInfo.aoId = aoId;
			playerInfo.position = birthPlace;
			playerInfo.playerId = playerId;
			playerInfo.configName = NetManager.Instance.server.playerManager.mainPlayer.configName;
			playerInfo.seed = NetManager.Instance.server.playerManager.mainPlayer.seed;
			connectionWork.InitPlayer(playerInfo);
			NetManager.Instance.server.playerManager.AddPlayer(connectionWork.player);

			//通知自己进入游戏，并返回一些配置
			ClientJoinGamePackage package = PackageFactory.GetPackage(PackageType.ClientJoinGame) as ClientJoinGamePackage;
			package.playerId = playerId;
			package.aoId = aoId;
			package.birthPlace = birthPlace;
			package.worldConfigName = NetManager.Instance.server.playerManager.mainPlayer.configName;
			package.seed = NetManager.Instance.server.playerManager.mainPlayer.seed;
			package.time = NetManager.Instance.server.gameTime;
			connectionWork.SendPackage(package);

			//通知其他人我已进入游戏，并出现在他们视野内
			PlayerJoinViewPackage joinViewPackage = PackageFactory.GetPackage(PackageType.PlayerJoinView) as PlayerJoinViewPackage;
			joinViewPackage.playerId = playerId;
			joinViewPackage.aoId = aoId;
			joinViewPackage.birthPlace = birthPlace;
			List<int> viewPlayers = connectionWork.player.viewPlayers;
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				otherPlayer.worker.SendPackage(joinViewPackage);
			}
		}
	}
}

