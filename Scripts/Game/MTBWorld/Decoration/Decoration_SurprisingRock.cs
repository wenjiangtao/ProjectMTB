using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class Decoration_SurprisingRock :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			int width = random.Range(1,3);
			int height = 6;
			for (int cy =y; cy < y + height; cy++) {
				for (int cx = -width; cx <= width; cx++) {
					for (int cz = -width; cz <= width; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			_spreadQueue.Clear();
			setBlock(chunk,x,y,z,new Block(BlockType.Block_11));
			_spreadQueue.Enqueue(new WorldPos(x,y,z));
			int spreadRate = 10;
			int heightSpreadRate = 40;
			int nextX;
			int nextY;
			int nextZ;
			while(_spreadQueue.Count > 0)
			{
				WorldPos pos = _spreadQueue.Dequeue();
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextX -= 1;
				if(nextX >= x - width && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < spreadRate)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextX += 1;
				if(nextX <= x + width && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < spreadRate)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextY -= 1;
				if(nextY >= y && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < spreadRate)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextY += 1;
				if(nextY < y + height && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < heightSpreadRate)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextZ -= 1;
				if(nextZ >= z - width && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < spreadRate)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
				nextX = pos.x;
				nextY = pos.y;
				nextZ = pos.z;
				nextZ += 1;
				if(nextZ <= z + width && chunk.GetBlock(nextX,nextY,nextZ).BlockType == BlockType.Air)
				{
					if(random.Range(0,100) < 60)
					{
						setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_11));
						_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
					}
				}
			}
			return true;
		}

		#endregion

		private Queue<WorldPos> _spreadQueue;
		public Decoration_SurprisingRock ()
		{
			_spreadQueue = new Queue<WorldPos>();
		}
	}
}

