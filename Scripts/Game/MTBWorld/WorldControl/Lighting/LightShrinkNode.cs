using System;
namespace MTB
{
	public struct LightShrinkNode
	{
		public int index;
		public int prevLightLevel;
		public int lightLevel;
		public Chunk chunk;
		
		public LightShrinkNode(int index,int prevLightLevel,int lightLevel,Chunk chunk)
		{
			this.index = index;
			this.prevLightLevel = prevLightLevel;
			this.lightLevel = lightLevel;
			this.chunk = chunk;
		}
	}
}

