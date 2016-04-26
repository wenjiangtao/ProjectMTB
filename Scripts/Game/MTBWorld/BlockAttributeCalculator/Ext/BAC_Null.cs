using System;
namespace MTB
{
	public class BAC_Null : BlockAttributeCalculator
	{
		public BAC_Null ()
		{
		}

		public override BlockType BlockType {
			get {
				return BlockType.Null;
			}
		}

//		public override MeshData BlockData(Section section, int x, int y, int z, MeshData meshData,byte extendId = 0,bool isInRange = false)
//		{
//			return meshData;
//		}

//		public override void CalculateMesh(Chunk chunk,int x,int y,int z,MeshData meshData,
//		                                  Block self,Block other,BlockAttributeCalculator otherCalculator,Direction direction)
//		{
//		}

		public override void CalculateMesh (Chunk chunk, MeshData meshData, Block self, Direction renderDirection, UnityEngine.Vector3 v1, UnityEngine.Vector3 v2, UnityEngine.Vector3 v3, UnityEngine.Vector3 v4,LightVertice sunLight,LightVertice blockLight)
		{

		}

		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.All;
		}

		public override MeshColliderType GetMeshColliderType (byte extendId)
		{
			return MeshColliderType.none;
		}
	}
}

