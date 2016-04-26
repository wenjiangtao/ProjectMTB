using System;
namespace MTB
{
	//表示当前场景中物块的一个实例
	public class SceneBlock
	{
		public WorldPos pos{get;private set;}
		public byte blockType{get;private set;}
		public byte extendId{get;private set;}
		public BlockData blockData{get;private set;}
		public int curHardness{get;set;}
		public SceneBlock (WorldPos pos,byte blockType,byte extendId)
		{
			this.pos = pos;
			this.blockType = blockType;
			this.extendId = extendId;
			blockData = BlockDataManager.Instance.GetBlockData(blockType,extendId);
			curHardness = blockData.hardness;
		}
	}
}

