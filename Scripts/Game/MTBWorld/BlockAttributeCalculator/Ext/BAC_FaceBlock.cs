using System;
using UnityEngine;
namespace MTB
{
	public abstract class BAC_FaceBlock : BlockAttributeCalculator
	{
		public override MeshColliderType GetMeshColliderType(byte extendId) {
			return MeshColliderType.supportCollider;
		}
		
		public override void CalculateMesh (Chunk chunk, MeshData meshData, Block self, Direction renderDirection, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,LightVertice sunLight,LightVertice blockLight)
		{
			
		}
		
		public override void CalculateSpecialMesh (Chunk chunk, int x, int y, int z, MeshData meshData, Block self, Block other, BlockAttributeCalculator otherCalculator, Direction direction)
		{
			//因为会渲染六次（六个正方向）
			if(direction == Direction.right)
			{
				meshData.useDoubleFace = true;
				FaceDataOne(chunk,x,y,z,meshData,self.ExtendId);
				meshData.useDoubleFace = MeshData.DefaultUseDoubleFace;
			}
		}
		
		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.Part;
		}
		
		public override int LightDamp (byte extendId)
		{
			return 0;
		}

		public override Direction GetFaceDirection (byte extendId)
		{
			if(extendId == 1 || extendId == 2)
			{
				return Direction.front;
			}
			return Direction.left;
		}

		public BAC_FaceBlock ()
		{
		}

		public virtual MeshData FaceDataOne
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{
			if(GetFaceDirection(extendId) == Direction.front)
			{
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z + 0.5f));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y,z + 0.5f));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + 1f,z + 0.5f));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + 1f,z + 0.5f));
			}
			else
			{
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 0.5f,y,z + 1f));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 0.5f,y,z));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 0.5f,y + 1f,z));
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 0.5f,y + 1f,z + 1f));
			}

			meshData.AddQuadTriangles();
			
			AddSpecialUV(meshData,extendId,Direction.up);
			
			AddSpecialColor(chunk,x,y,z,meshData);
			return meshData;
		}

	}
}

