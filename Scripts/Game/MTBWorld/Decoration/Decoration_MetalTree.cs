using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class Decoration_MetalTree :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			int treeTrunkWidth = 2;
			int treeTrunkHeight = random.Range(5,10);
			int treeHalfLeaveWidth = random.Range(2,5);
			int treeLeaveHeight = random.Range(2,5);
			int treeLeaveOffset = 0;
			if(!CheckCanGenerator(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeHalfLeaveWidth,treeLeaveHeight,treeLeaveOffset))
			{
				return false;
			}
			return CreateTree(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeHalfLeaveWidth,treeLeaveHeight,treeLeaveOffset,random);;
		}

		#endregion

		private Queue<WorldPos> _spreadQueue;
		public Decoration_MetalTree ()
		{
			_spreadQueue = new Queue<WorldPos>();
		}

		private bool CheckCanGenerator(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeHalfLeaveWidth,int treeLeaveHeight,int treeLeaveOffset)
		{
			int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
			int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
			for (int cx = x; cx < x + treeTrunkWidth; cx++) {
				for (int cz = z; cz < z + treeTrunkWidth; cz++) {
					if(chunk.GetBlock(cx,y - 1,cz).BlockType == BlockType.Air)
					{
						return false;
					}
				}
			}
			for (int cy = y; cy < y + treeHeight; cy++) {
				if(cy - y < treeCrownStartHeight)
				{
					for (int cx = x; cx < x + treeTrunkWidth; cx++) {
						for (int cz = z; cz < z + treeTrunkWidth; cz++) {
							if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
							{
								return false;
							}
						}
					}
					continue;
				}
				for (int cx = x - treeHalfLeaveWidth; cx <= x + treeHalfLeaveWidth + 1; cx++) {
					for (int cz = z - treeHalfLeaveWidth; cz <= z + treeHalfLeaveWidth + 1 ; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool CreateTree(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeHalfLeaveWidth,int treeLeaveHeight,int treeLeaveOffset,IMTBRandom random)
		{
			int treeLeaveStartHeight = y + treeTrunkHeight + treeLeaveOffset - 1;
			int treeTopHeight = treeLeaveStartHeight + treeLeaveHeight;

			_spreadQueue.Clear();
			for (int cx = x; cx < x + treeTrunkWidth; cx++) {
				for (int cz = z; cz < z + treeTrunkWidth; cz++) {
					setBlock(chunk,cx,treeLeaveStartHeight,cz,new Block(BlockType.Block_28));
				}
			}

			setBlock(chunk,x - 1,treeLeaveStartHeight,z,new Block(BlockType.Block_28));
			_spreadQueue.Enqueue(new WorldPos(x - 1,treeLeaveStartHeight,z));
			setBlock(chunk,x + 1,treeLeaveStartHeight,z - 1,new Block(BlockType.Block_28));
			_spreadQueue.Enqueue(new WorldPos(x + 1,treeLeaveStartHeight,z - 1));
			setBlock(chunk,x + 2,treeLeaveStartHeight,z + 1,new Block(BlockType.Block_28));
			_spreadQueue.Enqueue(new WorldPos(x + 2,treeLeaveStartHeight,z + 1));
			setBlock(chunk,x,treeLeaveStartHeight,z + 2,new Block(BlockType.Block_28));
			_spreadQueue.Enqueue(new WorldPos(x,treeLeaveStartHeight,z + 2));
		
			int nextX;
			int nextY;
			int nextZ;
			while(_spreadQueue.Count > 0)
			{
				WorldPos pos = _spreadQueue.Dequeue();
				int spreadNum = random.Range(0,4);
				for (int i = 0; i < spreadNum; i++) {
					int direct = random.Range(0,6);
					switch(direct)
					{
					case 0:
						nextX = pos.x - 1;
						nextY = pos.y;
						nextZ = pos.z;
						break;
					case 1:
						nextX = pos.x + 1;
						nextY = pos.y;
						nextZ = pos.z;
						break;
					case 2:
						nextX = pos.x;
						nextY = pos.y;
						nextZ = pos.z - 1;
						break;
					case 3:
						nextX = pos.x;
						nextY = pos.y;
						nextZ = pos.z + 1;
						break;
					default:
						nextX = pos.x;
						nextY = pos.y + 1;
						nextZ = pos.z;
						break;
					}
					if(nextX < x - treeHalfLeaveWidth || nextX > x + treeHalfLeaveWidth 
					   || nextZ < z - treeHalfLeaveWidth || nextZ > z + treeHalfLeaveWidth
					   || nextY >= treeTopHeight || chunk.GetBlock(nextX,nextY,nextZ).BlockType != BlockType.Air)
					{
						continue;
					}
//					int num = CheckAroundNum(chunk,nextX,nextY,nextZ);
//					if(num > 2)continue;
					setBlock(chunk,nextX,nextY,nextZ,new Block(BlockType.Block_28));
					_spreadQueue.Enqueue(new WorldPos(nextX,nextY,nextZ));
				}
			}
			for (int cy = y; cy < y + treeTrunkHeight - 1; cy++) {
				for (int cx = x; cx < x + treeTrunkWidth; cx++) {
					for (int cz = z; cz < z + treeTrunkWidth; cz++) {
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_28));
					}
				}
			}
				
			return true;
		}

		private int CheckAroundNum(Chunk chunk,int x,int y,int z)
		{
			int num = 0;
			if(chunk.GetBlock(x - 1,y,z).BlockType != BlockType.Air)num++;
			if(chunk.GetBlock(x + 1,y,z).BlockType != BlockType.Air)num++;
			if(chunk.GetBlock(x,y,z - 1).BlockType != BlockType.Air)num++;
			if(chunk.GetBlock(x,y,z + 1).BlockType != BlockType.Air)num++;
			if(chunk.GetBlock(x,y - 1,z).BlockType != BlockType.Air)num++;
			if(chunk.GetBlock(x,y + 1,z).BlockType != BlockType.Air)num++;
			return num;
		}

	}
}

