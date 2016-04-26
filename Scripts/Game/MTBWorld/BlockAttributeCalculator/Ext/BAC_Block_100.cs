using System;
namespace MTB
{
	public class BAC_Block_100 : BAC_ModelBlock
	{

		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_100;
			}
		}

		#endregion

		public override void Notify (World world, int x, int y, int z, Direction direction, Block oldBlock, Block newBlock)
		{
			if(oldBlock.BlockType == BlockType && newBlock.BlockType == BlockType.Air)
			{
				if(direction == Direction.up || direction == Direction.down)return;
				Block block = world.GetBlock(x,y,z);
				if((block.ExtendId & 7) == (oldBlock.ExtendId & 7) && (block.ExtendId & 8) != (oldBlock.ExtendId & 8))
				{
					BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType);
					dispatcher.SetBlock(world,x,y,z,new Block(BlockType.Air));
				}
			}
		}

		public override bool IsModelCenter (byte extendId)
		{
			return (extendId & 8) != 0;
		}

		public BAC_Block_100 ()
		{
		}
	}
}

