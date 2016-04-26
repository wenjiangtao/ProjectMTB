using System;
namespace MTB
{
	//与客户端连接的worker
	public class ClientConnectionWorker : ConnectionWorker
	{
		public ClientPlayer player{get;private set;}
		public ClientConnectionWorker (Connection conn)
			:base(conn)
		{
		}

		public void InitPlayer(ClientPlayerInfo playerInfo)
		{
			player = new ClientPlayer(playerInfo.id,this);
			player.aoId = playerInfo.aoId;
			player.playerId = playerInfo.playerId;
			player.InitPosition(playerInfo.position);
			player.configName = playerInfo.configName;
			player.seed = playerInfo.seed;
		}

		public override void Stop ()
		{
			base.Stop ();
		}

		public override void Dispose ()
		{
			if(player != null)
			{
				player.Dispose();
				//移除当前玩家
				NetManager.Instance.server.playerManager.RemovePlayer(player);
				//向其他玩家发送玩家退出消息
				LeaveGamePackage package = PackageFactory.GetPackage(PackageType.LeaveGame) as LeaveGamePackage;
				package.aoId = player.aoId;
				NetManager.Instance.server.playerManager.BroadcastPackage(package);
			}
			base.Dispose ();
		}
	}
}

