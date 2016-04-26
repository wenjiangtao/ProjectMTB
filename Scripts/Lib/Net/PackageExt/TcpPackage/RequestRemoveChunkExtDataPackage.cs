using System;
using System.Collections.Generic;
namespace MTB
{
	//请求将要移除的区块额外信息（删除时，获取到是否需要保存，以及将entity信息获取到）
	public class RequestRemoveChunkExtDataPackage : TcpPackage
	{
		public WorldPos pos{get;set;}

		public RequestRemoveChunkExtDataPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
		}

		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ResponseRemoveChunkExtDataPackage package = PackageFactory.GetPackage(PackageType.ResponseRemoveChunkExtData)
				as ResponseRemoveChunkExtDataPackage;
			package.pos = pos;
			package.needSave = NetManager.Instance.server.sceneManager.CanchangedChunkSaved(pos);
			package.entities = new List<ClientEntityInfo>();
			List<ClientEntity> list = NetManager.Instance.server.entityManager.GetEntitiesInChunk(pos);
			if(list != null)
			{
				for (int i = 0; i < list.Count; i++) {
					package.entities.Add(list[i].GetClientEntityInfo());
				}
			}
			connectionWork.SendPackage(package);
		}
	}
}

