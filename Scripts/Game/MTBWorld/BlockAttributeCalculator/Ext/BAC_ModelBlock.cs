using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public abstract class BAC_ModelBlock : BlockAttributeCalculator
	{
		private static Dictionary<Direction,Vector2> directionRotate;
		public BAC_ModelBlock ()
		{
			directionRotate = new Dictionary<Direction, Vector2>();
			directionRotate.Add(Direction.front,new Vector2(0,1));
			directionRotate.Add(Direction.left,new Vector2(-1,0));
			directionRotate.Add(Direction.back,new Vector2(0,-1));
			directionRotate.Add(Direction.right,new Vector2(1,0));
		}

		public virtual bool IsModelCenter(byte extendId){return true;}

		public override MeshColliderType GetMeshColliderType (byte extendId)
		{
			return MeshColliderType.none;
		}
		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.Part;
		}

		public override void CalculateMesh (Chunk chunk, MeshData meshData, Block self, Direction renderDirection, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,LightVertice sunLight,LightVertice blockLight)
		{
			
		}

		public override void CalculateSpecialMesh (Chunk chunk, int x, int y, int z, MeshData meshData, Block self, Block other, BlockAttributeCalculator otherCalculator, Direction direction)
		{
			//因为会计算六次
			if(direction != Direction.right)return;
			if(!IsModelCenter(self.ExtendId))return;
			ModelData modelData = ModelDataManager.Instance.GetModelData((int)self.BlockType);
			Direction face = GetFaceDirection(self.ExtendId);
			Vector2 rotate = GetRotateSinAndCos(face);
			int sunLight = chunk.GetSunLight(x,y,z,true);
			int blockLight = chunk.GetBlockLight(x,y,z,true); 
			Rect rect = GetUVRect(self.ExtendId,direction);
			meshData.useRenderDataForCol = true;
			int verticesIndex = meshData.GetCurVerticesIndex();
			int colVerticesIndex = meshData.GetCurColVerticesIndex();
			for (int i = 0; i < modelData.vertices.Length; i++) {
				Vector3[] vertices = modelData.vertices;
				Vector2[] uvs = modelData.uvs;

				float verX = vertices[i].x * rotate.y + vertices[i].z * rotate.x;
				float verZ = -vertices[i].x * rotate.x + vertices[i].z * rotate.y;
				float realX = x + 0.5f + verX;
				float realY = y + vertices[i].y;
				float realZ = z + 0.5f + verZ;
				//添加渲染网格
				meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(realX,realY,realZ));
				meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],0,2));
				//这里要转换uv坐标
				meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(rect.x + uvs[i].x * rect.width,rect.y + uvs[i].y * rect.height));

				//添加碰撞网格
				meshData.AddColVertice(MeshBaseDataCache.Instance.GetVector3(realX,realY,realZ));
			}
			for (int i = 0; i < modelData.triangles.Length; i++) {
				meshData.AddTriangle(verticesIndex + modelData.triangles[i]);

				meshData.AddColTriangle(colVerticesIndex + modelData.triangles[i]);
			}
			meshData.useRenderDataForCol = MeshData.DefaultUseRenderDataForCol;
		}

		private Vector2 GetRotateSinAndCos(Direction direction)
		{
			if(directionRotate.ContainsKey(direction))
			{
				return directionRotate[direction];
			}
			return new Vector2(0,1);
		}

		public override Direction GetFaceDirection (byte extendId)
		{
			int result = (extendId & 3);
			if(result == 1)return Direction.right;
			else if(result == 2) return Direction.back;
			else if(result == 3) return Direction.left;
			return Direction.front;
		}

		public override byte GetResourceExtendId (byte extendId)
		{
			return 0;
		}

		public override bool CanCombineWithBlock (byte extendId, Block block)
		{
			return false;
		}

		public override int LightDamp (byte extendId)
		{
			return 0;
		}
	}
}

