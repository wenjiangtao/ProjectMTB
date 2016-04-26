using System;
using UnityEngine;
namespace MTB
{
	public class Decoration_Block_50 : IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
//			int height = random.Range(1,3);
//			int i = 0;
//			for (i = 0; i < height; i++) {
//				if(chunk.GetBlock(x,y + i,z,true).BlockType != BlockType.Air)
//				{
//					break;
//				}
//			}
//			if(i == height)
//			{
//				for (i = 0; i < height; i++) {
//					chunk.SetBlock(x,y + i,z,new Block(BlockType.Block_50));
//				}
//				return true;
//			}
//			return false;

			Vector3[] posData = totalData[random.Range(0,totalData.Length)][random.Range(0,4)];
			for (int i = 0; i < posData.Length; i++) {
				if(chunk.GetBlock(x + (int)posData[i].x,y + (int)posData[i].y,z + (int)posData[i].z).BlockType != BlockType.Air)
				{
					return false;
				}
				if(posData[i].y == 0)
				{
					if(chunk.GetBlock(x + (int)posData[i].x,y + (int)posData[i].y - 1,z + (int)posData[i].z).BlockType == BlockType.Air)
					{
						return false;
					}
				}
			}
			for (int i = 0; i < posData.Length; i++) {
				chunk.SetBlock(x + (int)posData[i].x,y + (int)posData[i].y,z + (int)posData[i].z,new Block(BlockType.Block_50));	
			}

			return false;
		}

		#endregion

		private Vector3[][] data = new Vector3[][]{
			new Vector3[]{
			new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(0,2,0),new Vector3(0,3,0),
			new Vector3(1,0,0),new Vector3(1,1,0),new Vector3(1,2,0),new Vector3(1,3,0),
			new Vector3(0,4,0),new Vector3(0,5,0),
			new Vector3(0,3,-1),new Vector3(0,3,-2),new Vector3(0,4,-2),
			new Vector3(1,3,1),new Vector3(2,3,1),new Vector3(2,4,1),
			new Vector3(0,3,1),new Vector3(0,4,1)
			},
			new Vector3[]{
				new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(0,2,0),new Vector3(0,3,0),new Vector3(0,4,0),new Vector3(0,5,0),
				new Vector3(1,0,0),new Vector3(1,1,0),new Vector3(1,2,0),new Vector3(1,3,0),new Vector3(1,4,0),new Vector3(1,5,0),
				new Vector3(1,6,0),new Vector3(1,7,0),
				new Vector3(0,5,-1),new Vector3(0,5,-2),new Vector3(0,6,-2),
				new Vector3(1,5,1),new Vector3(2,5,1),new Vector3(2,6,1),
				new Vector3(0,5,1),new Vector3(0,6,1)
			}
		};

		private Vector3[][][] totalData;

		private Vector3[] GetData(Vector3[] d,int rotateDegree)
		{
			Vector3[] result = new Vector3[d.Length];
			switch(rotateDegree)
			{
			case 1:
				for (int i = 0; i < d.Length; i++) {
					result[i] = new Vector3(d[i].z,d[i].y,d[i].x);
				}
				break;
			case 2:
				for (int i = 0; i < d.Length; i++) {
					result[i] = new Vector3(-d[i].x,d[i].y,d[i].z);
				}
				break;
			case 3:
				for (int i = 0; i < d.Length; i++) {
					result[i] = new Vector3(d[i].z,d[i].y,-d[i].x);
				}
				break;
			default:
				for (int i = 0; i < d.Length; i++) {
					result[i] = d[i];
				}
				break;
			}
			return result;
		}

		public Decoration_Block_50 ()
		{
			totalData = new Vector3[data.Length][][];
			for (int i = 0; i < data.Length; i++) {
				totalData[i] = new Vector3[4][];
				Vector3[][] directionData = totalData[i];
				for (int j = 0; j < 4; j++) {
					directionData[j] = GetData(data[i],j);
				}
			}

		}
	}
}

