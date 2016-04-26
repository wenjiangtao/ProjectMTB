using System;
namespace MTB
{
	public struct LightSpreadNode
	{
		public int index;
		public int lightLevel;
		public Chunk chunk;
		
		public LightSpreadNode(int index,int level,Chunk chunk)
		{
			this.index = index;
			this.lightLevel = level;
			this.chunk = chunk;
		}
	}
}

