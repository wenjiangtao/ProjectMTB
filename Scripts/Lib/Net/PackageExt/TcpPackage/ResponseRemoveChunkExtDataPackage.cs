using System;
using System.Collections.Generic;
namespace MTB
{
	//服务器返回将要移除的区块额外信息（删除时，获取到是否需要保存，以及将entity信息获取到）
	public class ResponseRemoveChunkExtDataPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public bool needSave{get;set;}
		public List<ClientEntityInfo> entities{get;set;}
		public ResponseRemoveChunkExtDataPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteBool(needSave);
			WriteInt(entities.Count);
			for (int i = 0; i < entities.Count; i++) {
				WriteClientEntityInfo(entities[i]);
			}
		}
		
		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			needSave = ReadBool();
			int count = ReadInt();
			entities = new List<ClientEntityInfo>();
			for (int i = 0; i < count; i++) {
				entities.Add(ReadClientEntityInfo());
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

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			EventManager.SendEvent(NetEventMacro.ON_RESPONSE_REMOVE_CHUNK_EXT_DATA,pos,needSave,entities);
		}
	}
}

