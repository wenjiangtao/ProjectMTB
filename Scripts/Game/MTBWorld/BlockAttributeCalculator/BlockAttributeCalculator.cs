using System;
using UnityEngine;
namespace MTB
{
	public abstract class BlockAttributeCalculator : IBlockNotify
	{

		#region IBlockNotify implementation

		public virtual void Notify (World world, int x, int y, int z,Direction direction, Block oldBlock, Block newBlock)
		{
		}

		public virtual void NotifySelf (World world, int x, int y, int z, Block oldBlock, Block newBlock)
		{
		}

		#endregion

		public BlockAttributeCalculator()
		{
		}

		public abstract BlockType BlockType{get;}

		public virtual MeshColliderType GetMeshColliderType(byte extendId){return MeshColliderType.terrainCollider;}
		public virtual BlockRenderType GetBlockRenderType(byte extendId){return BlockRenderType.All;}

		public virtual void CalculateMesh(Chunk chunk,MeshData meshData,Block self,Direction renderDirection,
		                                  Vector3 v1,Vector3 v2,Vector3 v3,Vector3 v4,
		                                  LightVertice sunLight,LightVertice blockLight)
		{
			meshData.AddVertice(v1);
			meshData.AddVertice(v2);
			meshData.AddVertice(v3);
			meshData.AddVertice(v4);
			if((int)renderDirection > 0)
			{
				meshData.AddQuadTriangles(true);
			}
			else
			{
				meshData.AddQuadTriangles(false);
			}
			AddRenderUV(meshData,self.ExtendId,renderDirection);

			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(sunLight.v00,blockLight.v00,0,1));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(sunLight.v01,blockLight.v01,0,1));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(sunLight.v11,blockLight.v11,0,1));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(sunLight.v10,blockLight.v10,0,1));
		}

		protected virtual void AddRenderUV(MeshData meshData,byte extendId,Direction direction)
		{
			Direction resultDirection = GetRealDirection(extendId,direction);
			
			Rect uvRect = GetUVRect(extendId,resultDirection);
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x,uvRect.y));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x,uvRect.y));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x,uvRect.y));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x,uvRect.y));
		}

		//特殊网格计算（不能确定当前坐标会在chunk中）
		public virtual void CalculateSpecialMesh(Chunk chunk,int x,int y,int z,MeshData meshData,Block self
		                                         ,Block other,BlockAttributeCalculator otherCalculator,Direction direction)
		{

		}

		public virtual bool CanCalculateMesh(Block self,Block other,BlockAttributeCalculator otherCalculator,Direction direction)
		{
			if(GetBlockRenderType(self.ExtendId) == BlockRenderType.All &&
			   otherCalculator.GetBlockRenderType(other.ExtendId) != BlockRenderType.All)
			{
				return true;
			}
			return false;
		}

		protected Direction GetRealDirection(byte extendId,Direction direction)
		{
			Direction faceDirection = GetFaceDirection(extendId);
			Direction resultDirection;
			switch(direction)
			{
			case Direction.front:
				switch(faceDirection)
				{
				case Direction.right:
					resultDirection = Direction.left;
					break;
				case Direction.back:
					resultDirection = Direction.back;
					break;
				case Direction.left:
					resultDirection = Direction.right;
					break;
				default:
					resultDirection = direction;
					break;
				}
				break;
			case Direction.right:
				switch(faceDirection)
				{
				case Direction.right:
					resultDirection = Direction.front;
					break;
				case Direction.back:
					resultDirection = Direction.left;
					break;
				case Direction.left:
					resultDirection = Direction.back;
					break;
				default:
					resultDirection = direction;
					break;
				}
				break;
			case Direction.back:
				switch(faceDirection)
				{
				case Direction.right:
					resultDirection = Direction.right;
					break;
				case Direction.back:
					resultDirection = Direction.front;
					break;
				case Direction.left:
					resultDirection = Direction.left;
					break;
				default:
					resultDirection = direction;
					break;
				}
				break;
			case Direction.left:
				switch(faceDirection)
				{
				case Direction.right:
					resultDirection = Direction.back;
					break;
				case Direction.back:
					resultDirection = Direction.right;
					break;
				case Direction.left:
					resultDirection = Direction.front;
					break;
				default:
					resultDirection = direction;
					break;
				}
				break;
			default:
				resultDirection = direction;
				break;
			}
			return resultDirection;
		}

		protected virtual void AddSpecialUV(MeshData meshData,byte extendId,Direction direction)
		{
			Direction resultDirection = GetRealDirection(extendId,direction);

			Rect uvRect = GetUVRect(extendId,resultDirection);
			float scale = uvRect.width * 0.03f;
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x + scale,uvRect.y + scale));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x + uvRect.width - scale,uvRect.y + scale));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x + uvRect.width - scale,uvRect.y + uvRect.height - scale));
			meshData.AddUV(MeshBaseDataCache.Instance.GetVector2(uvRect.x + scale, uvRect.y + uvRect.height - scale));
		}

		public virtual Rect GetUVRect(byte extendId,Direction direction)
		{
			byte resExtendId = GetResourceExtendId(extendId);
			if(WorldTextureProvider.IsCanUseProvider)
			{
				return WorldTextureProvider.Instance.GetBlockTextureRect(BlockType,resExtendId,direction);
			}
			throw new Exception("未初始化贴图资源");
		}

		public virtual byte GetResourceExtendId(byte extendId)
		{
			return extendId;
		}

		protected virtual void AddSpecialColor(Chunk chunk,int x,int y,int z,MeshData meshData)
		{
			int sunLight = chunk.GetSunLight(x,y,z,true);
			int blockLight = chunk.GetBlockLight(x,y,z,true); 
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],0,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],0,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],0,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],0,2));
		}

		protected int Average(int center,int b,int c,int d)
		{
			int average = ((center) + (b) + (c) + (d)) >> 2;
			return average;
		}

		public virtual Direction GetFaceDirection(byte extendId)
		{
			return Direction.front;
		}

		public virtual bool CanCombineWithBlock(byte extendId,Block block)
		{
			if(block.BlockType == BlockType && extendId == block.ExtendId)return true;
			return false;
		}

		public virtual int LightDamp(byte extendId)
		{
			return int.MaxValue;
		}

		public virtual int LightLevel(byte extendId)
		{
			return 0;
		}
	}

	public enum BlockRenderType
	{
		//不渲染，像空气、null
		None,
		//渲染一部分，如交叉物块
		Part,
		//6个面都能渲染
		All
	}

	public struct LightVertice
	{
		public float v00;
		public float v01;
		public float v11;
		public float v10;

		public override bool Equals (object obj)
		{
			if(obj is LightVertice)
			{
				LightVertice other = (LightVertice)obj;
				return (v00 == other.v00 && v01 == other.v01 && v11 == other.v11 && v10 == other.v10);
			}
			return false;
		}

		public bool EqualOther(LightVertice other)
		{
			return (v00 == other.v00 && v01 == other.v01 && v11 == other.v11 && v10 == other.v10);
		}
	}
}

