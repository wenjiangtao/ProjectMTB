using System;
namespace MTB
{
	public class BlockDispatcher
	{
		public BlockDispatcher ()
		{
		}

		public virtual void SetBlock(World world,int x,int y,int z,Block block,bool notify = true)
		{
			if(!IsHeightLimit(y))return;
			SetBlockAndNotify(world,x,y,z,block,notify);
			world.CheckAndRecalculateMesh(x,y,z,block);
		}

		public bool IsHeightLimit(int y)
		{
			if(y <= WorldConfig.Instance.heightCap && y >= 0)
				return true;
			return false;
		}

		public void SetBlockAndNotify(World world,int x,int y,int z,Block block,bool notify = true)
		{
			Block oldBlock = world.GetBlock(x,y,z);
			world.SetBlock(x,y,z,block);
			//当有玩家对物块进行操作时，同步物块数据
			Chunk chunk = world.GetChunk(x,y,z);
			if(chunk != null && chunk.isPopulationDataPrepared)
			{
				int tempX = x - chunk.worldPos.x;
				int tempY = y - chunk.worldPos.y;
				int tempZ = z - chunk.worldPos.z;
				EventManager.SendEvent(EventMacro.BLOCK_DATA_UPDATE_AFTER_POPULATION,chunk.worldPos,tempX,tempY,tempZ,block);
			}
			if(notify)
			{
				IBlockNotify oldNotify = BlockAttributeCalculatorFactory.GetCalculator(oldBlock.BlockType);
				oldNotify.NotifySelf(world,x,y,z,oldBlock,block);
				NotifyAround(world,x,y,z,oldBlock,block);
			}
		}

		public void NotifyAround(World world,int x,int y,int z,Block oldBlock,Block newBlock)
		{
			IBlockNotify leftNotify = GetBlockNotify(world,x - 1,y,z);
			leftNotify.Notify(world,x - 1,y,z,Direction.right,oldBlock,newBlock);

			IBlockNotify rightNotify = GetBlockNotify(world,x + 1,y,z);
			rightNotify.Notify(world,x + 1,y,z,Direction.left,oldBlock,newBlock);

			IBlockNotify frontNotify = GetBlockNotify(world,x,y,z + 1);
			frontNotify.Notify(world,x,y,z + 1, Direction.back,oldBlock,newBlock);

			IBlockNotify backNotify = GetBlockNotify(world,x,y,z - 1);
			backNotify.Notify(world,x,y,z - 1,Direction.front,oldBlock,newBlock);

			IBlockNotify upNotify = GetBlockNotify(world,x,y + 1,z);
			upNotify.Notify(world,x,y + 1,z,Direction.down,oldBlock,newBlock);

			IBlockNotify downNotify = GetBlockNotify(world,x,y - 1,z);
			downNotify.Notify(world,x,y - 1,z,Direction.up,oldBlock,newBlock);
		}

		private IBlockNotify GetBlockNotify(World world,int x,int y,int z)
		{
			Block b = world.GetBlock(x,y,z);
			return BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
		}
	}
}

