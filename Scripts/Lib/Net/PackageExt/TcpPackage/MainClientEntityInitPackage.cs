using System;
using System.Collections.Generic;
namespace MTB
{
	public class MainClientEntityInitPackage : TcpPackage
	{
		public List<ClientEntityInfo> entityInfos{get;set;}

		public MainClientEntityInitPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(entityInfos.Count);
			for (int i = 0; i < entityInfos.Count; i++) {
				WriteClientEntityInfo(entityInfos[i]);
			}
		}

		public override void ReadAllData ()
		{
			entityInfos = new List<ClientEntityInfo>();
			int count = ReadInt();
			for (int i = 0; i < count; i++) {
				entityInfos.Add(ReadClientEntityInfo());
			}
		}

		private void WriteClientEntityInfo(ClientEntityInfo info)
		{
			WriteInt(info.aoId);
			WriteInt(info.entityId);
			WriteInt(info.type);
			WriteInt(info.roleId);
			WriteListInt(info.extData);
			WriteVector3(info.position);
		}

		private ClientEntityInfo ReadClientEntityInfo()
		{
			ClientEntityInfo info = new ClientEntityInfo();
			info.aoId = ReadInt();
			info.entityId = ReadInt();
			info.type = ReadInt();
			info.roleId = ReadInt();
			info.extData = ReadListInt();
			info.position = ReadVector3();
			return info;
		}

		private void WriteListInt(List<int> list)
		{
			WriteInt(list.Count);
			for (int i = 0; i < list.Count; i++) {
				WriteInt(list[i]);
			}
		}
		
		private List<int> ReadListInt()
		{
			List<int> list = new List<int>();
			int count = ReadInt();
			for (int i = 0; i < count; i++) {
				list.Add(ReadInt());
			}
			return list;
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			for (int i = 0; i < entityInfos.Count; i++) {
				NetManager.Instance.server.entityManager.InitEntity(entityInfos[i]);
			}
			//可是entity全部交由服务器来管理，因此，客户端的某些怪物可能需要移掉(不在视野范围之内)
			for (int i = 0; i < entityInfos.Count; i++) {
				ClientEntity clientEntity = NetManager.Instance.server.entityManager.GetEntity(entityInfos[i].aoId);
				//检测归属变更
				clientEntity.CheckViewHold();
				if(clientEntity.viewPlayers.Count <= 0)
				{
					EntityLeaveViewPackage leaveViewPackage = PackageFactory.GetPackage(PackageType.EntityLeaveView) as EntityLeaveViewPackage;
					leaveViewPackage.aoId = clientEntity.aoId;
					leaveViewPackage.type = clientEntity.type;
					connectionWork.SendPackage(leaveViewPackage);
				}
			}
		}
	}
}

