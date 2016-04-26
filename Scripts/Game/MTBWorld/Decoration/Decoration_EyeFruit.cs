using System;
namespace MTB
{
	public class Decoration_EyeFruit :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			if(chunk.GetBlock(x,y,z).BlockType != BlockType.Air || chunk.GetBlock(x,y + 1,z).BlockType != BlockType.Air)
			{
				return false;
			}
			if(random.Range(0,2) < 1)
			{
				setBlock(chunk,x,y,z,new Block(BlockType.Block_87,1));
				setBlock(chunk,x,y+1,z,new Block(BlockType.Block_87,2));
			}
			else
			{
				setBlock(chunk,x,y,z,new Block(BlockType.Block_87,3));
				setBlock(chunk,x,y+1,z,new Block(BlockType.Block_87,4));
			}
			return true;
		}

		#endregion

		public Decoration_EyeFruit ()
		{
            _decorationType = DecorationType.EyeFruit;
		}
	}
}

