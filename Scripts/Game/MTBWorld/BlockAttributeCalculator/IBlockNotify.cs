using System;
namespace MTB
{
	public interface IBlockNotify
	{
		void Notify(World world,int x,int y,int z,Direction direction, Block oldBlock,Block newBlock);
		void NotifySelf(World world,int x,int y,int z,Block oldBlock,Block newBlock);
	}
}

