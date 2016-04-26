using System;
using UnityEngine;
namespace MTB
{
	//交叉的物块
	public abstract class BAC_CrossBlock : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

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
				FaceDataTwo(chunk,x,y,z,meshData,self.ExtendId);
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

		#endregion

		public BAC_CrossBlock ()
		{
		}

		public virtual MeshData FaceDataOne
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + 1f,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + 1f,z + 1f));
			
			meshData.AddQuadTriangles();
			
			AddSpecialUV(meshData,extendId,Direction.up);

//			AddColor32(chunk,x,y,z,meshData);
			AddSpecialColor(chunk,x,y,z,meshData);
			return meshData;
		}
		
		public virtual MeshData FaceDataTwo
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + 1f,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + 1f,z));
			
			meshData.AddQuadTriangles();
			
			AddSpecialUV(meshData,extendId,Direction.up);

//			AddColor32(chunk,x,y,z,meshData);
			AddSpecialColor(chunk,x,y,z,meshData);
			return meshData;
		}
	}
}

