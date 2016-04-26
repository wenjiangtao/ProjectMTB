using System;
using UnityEngine;
namespace MTB
{
	public class BAC_StillWater : BlockAttributeCalculator
	{

		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.StillWater;
			}
		}

		public override void Notify (World world, int x, int y, int z, Direction direction, Block oldBlock, Block newBlock)
		{
//			if(direction != Direction.up && newBlock.BlockType == BlockType.Air)
//			{
//				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType.FlowingWater);
//				dispatcher.SetBlock(world,x,y,z,new Block(BlockType.FlowingWater,0));
//			}
		}

		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.Part;
		}

		public override bool CanCalculateMesh (Block self, Block other, BlockAttributeCalculator otherCalculator, Direction direction)
		{
			if(other.BlockType == BlockType.Air)
			{
				return true;
			}
			return false;
		}

		public override MeshColliderType GetMeshColliderType(byte extendId) {
			return MeshColliderType.none;
		}

		public override void CalculateMesh (Chunk chunk, MeshData meshData, Block self, Direction renderDirection, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,LightVertice sunLight,LightVertice blockLight)
		{
			meshData.useTransparentTexture = true;
			meshData.useDoubleFace = true;
			base.CalculateMesh (chunk, meshData, self, renderDirection, v1, v2, v3, v4,sunLight,blockLight);
			meshData.useTransparentTexture = MeshData.DefaultUseTransparentTexture;
			meshData.useDoubleFace = MeshData.DefaultUseDoubleFace;
		}

//		public override void CalculateSpecialMesh (Chunk chunk, int x, int y, int z, MeshData meshData, Block self, Block other, BlockAttributeCalculator otherCalculator, Direction direction)
//		{
//			if(other.BlockType == BlockType.Air)
//			{
//				meshData.useTransparentTexture = true;
//				meshData.useDoubleFace = true;
//				CalculatorWaterDirection(chunk,x,y,z,meshData,direction,1f,self.ExtendId);
//				meshData.useTransparentTexture = MeshData.DefaultUseTransparentTexture;
//				meshData.useDoubleFace = MeshData.DefaultUseDoubleFace;
//			}
//		}

//		protected void CalculatorWaterDirection (Chunk chunk, int x, int y, int z, MeshData meshData, Direction direction,float blockHeight,byte extendId)
//		{
//			switch(direction)
//			{
//			case Direction.back:
//				FaceDataBack(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			case Direction.front:
//				FaceDataFront(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			case Direction.up:
//				FaceDataUp(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			case Direction.down:
//				FaceDataDown(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			case Direction.left:
//				FaceDataLeft(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			default:
//				FaceDataRight(chunk,x,y,z,meshData,blockHeight,extendId);
//				break;
//			}
//		}

		public override Rect GetUVRect(byte extendId,Direction direction)
		{
			Rect uvRect = new Rect(0,0,0,0);
			if(WorldTextureProvider.IsCanUseProvider)
			{
				uvRect = WorldTextureProvider.Instance.GetBlockTextureRect(BlockType,0,direction);
			}
			return uvRect;
		}

		#endregion

		public BAC_StillWater ()
		{
		}

		public override int LightDamp (byte extendId)
		{
			return 1;
		}

//		public MeshData FaceDataUp
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			
//			meshData.AddQuadTriangles();
//			
//			AddSpecialUV(meshData,extendId,Direction.up);
//
////			AddColor32Up(chunk,x,y,z,meshData);
//
//			return meshData;
//		}
//		
//		public virtual MeshData FaceDataDown
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			return base.FaceDataDown(chunk,x,y,z,meshData,extendId);
//		}
//		
//		public MeshData FaceDataBack
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f,z - 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f,z - 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			
//			meshData.AddQuadTriangles();
//			
//			AddSpecialUV(meshData,extendId,Direction.back);
//
////			AddColor32Back(chunk,x,y,z,meshData);
//
//			return meshData;
//		}
//		
//		public MeshData FaceDataLeft
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f,z - 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f,z + 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			
//			meshData.AddQuadTriangles();
//			
//			AddSpecialUV(meshData,extendId,Direction.left);
//
////			AddColor32Left(chunk,x,y,z,meshData);
//
//			return meshData;
//		}
//		
//		public MeshData FaceDataRight
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f,z + 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f,z - 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z - 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			
//			meshData.AddQuadTriangles();
//			
//			AddSpecialUV(meshData,extendId,Direction.right);
//
////			AddColor32Right(chunk,x,y,z,meshData);
//
//			return meshData;
//		}
//		
//		public MeshData FaceDataFront
//			(Chunk chunk, int x, int y, int z, MeshData meshData,float blockHeight,byte extendId)
//		{
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f,z + 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f,z + 0.5f));
//			meshData.AddVertice(new Vector3(x + 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			meshData.AddVertice(new Vector3(x - 0.5f,y - 0.5f + blockHeight,z + 0.5f));
//			
//			meshData.AddQuadTriangles();
//			
//			AddSpecialUV(meshData,extendId,Direction.front);
//
////			AddColor32Front(chunk,x,y,z,meshData);
//
//			return meshData;
//		}
//
	}
}

