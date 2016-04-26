using System;
namespace MTB
{
	//玩家chunk数据产生时，应向服务器发送chunk添加消息
	public class PlayerAddChunkPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public int sign{get;set;}
		public PlayerAddChunkPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteInt(sign);
		}

		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			sign = ReadInt();
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			//将当前玩家的chunk更新数据添加到服务器上
			NetManager.Instance.server.sceneManager.AddPlayerInChunks(connectionWork.player,pos,sign);
		}
	}
}

