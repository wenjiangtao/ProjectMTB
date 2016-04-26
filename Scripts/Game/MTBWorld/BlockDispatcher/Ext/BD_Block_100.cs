using System;
using System.Collections.Generic;
namespace MTB
{
	public class BD_Block_100 : BlockDispatcher
	{
		private Dictionary<Direction,WorldPos> nextBlockOffset;
		public override void SetBlock (World world, int x, int y, int z, Block block,bool notify = true)
		{
			if(!IsHeightLimit(y))return;
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(block.BlockType);
			Direction direction = calculator.GetFaceDirection(block.ExtendId);
			WorldPos offset = new WorldPos();
			if(direction == Direction.front)offset = new WorldPos(x - 1,y,z);
			if(direction == Direction.back)offset = new WorldPos(x + 1,y,z);
			if(direction == Direction.left)offset = new WorldPos(x,y,z - 1);
			if(direction == Direction.right)offset = new WorldPos(x,y,z + 1);
			byte extendId = (byte)(block.ExtendId & 7);
			if(world.GetBlock(offset.x,offset.y,offset.z).BlockType != BlockType.Air)return;
			Block tempBlock = new Block(block.BlockType,extendId);
			SetBlockAndNotify(world,offset.x,offset.y,offset.z,tempBlock);
			SetBlockAndNotify(world,x,y,z,block);
			world.CheckAndRecalculateMesh(offset.x,offset.y,offset.z,tempBlock);
			world.CheckAndRecalculateMesh(x,y,z,block);

		}

		public BD_Block_100 ()
		{
		}
	}
}

