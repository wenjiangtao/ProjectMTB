using System;
using System.Collections.Generic;
namespace MTB
{
    public class ChunkAreaChangedPackage : TcpPackage
    {
        public RefreshChunkArea area { get; set; }

        public ChunkAreaChangedPackage(int id)
            : base(id)
        {
        }

        public override void WriteAllData()
        {
            WriteInt(area.chunkList.Count);
            for (int i = 0; i < area.chunkList.Count; i++)
            {
                WriteChangedChunk(area.chunkList[i]);
            }
        }

        public override void ReadAllData()
        {
            int count = ReadInt();
            List<RefreshChunk> chunkList = new List<RefreshChunk>(count);

            for (int i = 0; i < count; i++)
            {
                chunkList.Add(readChangedChunk());
            }
            area = new RefreshChunkArea(chunkList);
        }

        private void WriteChangedChunk(RefreshChunk chunk)
        {
            WriteInt(chunk.chunk.worldPos.x);
            WriteInt(chunk.chunk.worldPos.y);
            WriteInt(chunk.chunk.worldPos.z);
            WriteInt(chunk.refreshList.Count);
            for (int i = 0; i < chunk.refreshList.Count; i++)
            {
                writeChangedBlock(chunk.refreshList[i]);
            }
        }

        private RefreshChunk readChangedChunk()
        {
            int x = ReadInt();
            int y = ReadInt();
            int z = ReadInt();
            int count = ReadInt();
            RefreshChunk chunk = new RefreshChunk(World.world.GetChunk(x, y, z));
            List<UpdateBlock> list = new List<UpdateBlock>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(readChangedBlock());
            }
			chunk.refreshList = list;
            return chunk;
        }

        private void writeChangedBlock(UpdateBlock block)
        {
            WriteInt(block.Pos.x);
            WriteInt(block.Pos.y);
            WriteInt(block.Pos.z);
            WriteByte((byte)block.type);
            WriteByte(block.exid);
        }

        private UpdateBlock readChangedBlock()
        {
            int x = ReadInt();
            int y = ReadInt();
            int z = ReadInt();
            WorldPos pos = new WorldPos(x, y, z);
            BlockType type = (BlockType)ReadByte();
            byte exid = ReadByte();

            return new UpdateBlock(type, exid, pos);
        }

        public override void ServerDo(ClientConnectionWorker connectionWork)
        {
            NetManager.Instance.server.sceneManager.ChangedArea(area);
            NetManager.Instance.server.sceneManager.BroadcastPlaerAreaChangedPackage(connectionWork.player, area, this, true);
            //UnityEngine.Debug.Log("收到更改chunk:" + pos.ToString() + " block:" + changedBlock.index + " " + changedBlock.blockType);
        }

        public override void ClientDo(ServerConnectionWorker connectionWork)
        {
            //World.world.WorldGenerator.RefreshChunk(area);
            EventManager.SendEvent(NetEventMacro.ON_NET_AREA_UPDATE, area);
            UnityEngine.Debug.Log("收到更改Area" + area.chunkList.Count + " pos:" + area.chunkList[0].chunk.worldPos.x + "," + area.chunkList[0].chunk.worldPos.y + "," + area.chunkList[0].chunk.worldPos.z);
        }
    }
}
