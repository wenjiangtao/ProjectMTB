using System;
namespace MTB
{
	public class BAC_Block_87 : BAC_FaceBlock
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_87;
			}
		}

		public override void Notify (World world, int x, int y, int z, Direction direction, Block oldBlock, Block newBlock)
		{
			if(direction == Direction.up || direction == Direction.down)
			{
				if(oldBlock.BlockType == BlockType.Block_87)
				{
					BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType);
					dispatcher.SetBlock(world,x,y,z,new Block(BlockType.Air));
				}
			}
		}

		public override byte GetResourceExtendId (byte extendId)
		{
			if(extendId > 2)extendId -= 2;
			return extendId;
		}
		#endregion

		public BAC_Block_87 ()
		{
		}
	}
}

