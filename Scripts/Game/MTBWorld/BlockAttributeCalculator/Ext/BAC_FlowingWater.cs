using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class BAC_FlowingWater : BlockAttributeCalculator
	{
		public static int waterExtendLevel = 7;
		#region implemented abstract members of BAC_FlowingWater
		
		public override BlockType BlockType {
			get {
				return BlockType.FlowingWater;
			}
		}

		public override void Notify (World world, int x, int y, int z, Direction direction, Block oldBlock, Block newBlock)
		{
			if(direction == Direction.up)return;

			Block cur = world.GetBlock(x,y,z);

			Block left = world.GetBlock(x - 1,y,z);
			Block right = world.GetBlock(x + 1,y,z);
			Block front = world.GetBlock(x,y,z + 1);
			Block back = world.GetBlock(x,y,z - 1);
			Block down = world.GetBlock(x,y - 1,z);
			Block up = world.GetBlock(x,y + 1,z);

			if(newBlock.BlockType == BlockType.Air)
			{
				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType);
				if(direction == Direction.down)
				{
					byte nextExtendId = cur.ExtendId > waterExtendLevel ? cur.ExtendId : (byte)(cur.ExtendId + waterExtendLevel + 1);
					dispatcher.SetBlock(world,x,y - 1,z,new Block(BlockType,nextExtendId));
				}
				else
				{
					int curLevel = GetWaterLevel(cur);
					//如果当前块的周围有比我更强的水流,那么扩展
					if(curLevel == 0 || GetWaterLevel(left) < curLevel || GetWaterLevel(right) < curLevel
					   || GetWaterLevel(front) < curLevel || GetWaterLevel(back) < curLevel)
						dispatcher.SetBlock(world,x,y,z,cur);
				}
			}

//			if(!IsAroundNoFindWater(left,right,front,back,up,down))
//			{
//				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType.Air);
//				dispatcher.SetBlock(world,x,y,z,new Block(BlockType.Air));
//				return;
//			}

//			if(IsCanChangeToStillSourceWater(left,right,front,back))
//			{
//				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType.StillWater);
//				dispatcher.SetBlock(world,x,y,z,new Block(BlockType.StillWater));
//			}
		}

		private int GetWaterLevel(Block b)
		{
			if(b.BlockType == BlockType)
			{
				int level = b.ExtendId > BAC_FlowingWater.waterExtendLevel ? 0 : (int)b.ExtendId;
				return level;
			}
			return int.MaxValue;
		}

		public override void NotifySelf (World world, int x, int y, int z, Block oldBlock, Block newBlock)
		{
			//重新流动的时候不收缩
			if(oldBlock.BlockType == BlockType.FlowingWater && newBlock.BlockType == BlockType.FlowingWater)return;
			LiquidShrinkSource source = LiquidShrinkSourceManager.GetShrinkSource();
			source.chunk = world.GetChunk(x,y,z);
			source.liquidType = oldBlock.BlockType;
			source.pos = new WorldPos(x - source.chunk.worldPos.x,y - source.chunk.worldPos.y,z - source.chunk.worldPos.z);
			source.curShrinkLevel = oldBlock.ExtendId > BAC_FlowingWater.waterExtendLevel ? 0 : (int)oldBlock.ExtendId;
			source.shrinkMaxLevel = BAC_FlowingWater.waterExtendLevel;
			source.OnShrinkPlaneFinish += HandleOnShrinkPlaneFinish;
			source.StartShrink();
		}

		void HandleOnShrinkPlaneFinish (LiquidShrinkSource source)
		{
			source.OnShrinkPlaneFinish -= HandleOnShrinkPlaneFinish;
			List<WorldPos> list = source.GetNextSpreadSourceList();
			for (int i = 0; i < list.Count; i++) {
				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(BlockType.Air);
				dispatcher.SetBlock(source.chunk.world,list[i].x,list[i].y,list[i].z,new Block(BlockType.Air));
			}
			LiquidShrinkSourceManager.SaveShrinkSource(source);
		}

		public override void CalculateSpecialMesh (Chunk chunk, int x, int y, int z, MeshData meshData, Block self, Block other, BlockAttributeCalculator otherCalculator, Direction direction)
		{
			if(other.BlockType == BlockType.Air || (direction == Direction.up && other.BlockType != BlockType.FlowingWater && other.BlockType != BlockType.StillWater)
			   || otherCalculator.GetBlockRenderType(other.ExtendId) == MTB.BlockRenderType.Part && other.BlockType != BlockType.FlowingWater && other.BlockType != BlockType.StillWater)
			{
				meshData.useTransparentTexture = true;
				meshData.useDoubleFace = true;
				CalculatorDirection(chunk,x,y,z,meshData,self.ExtendId,direction);
				meshData.useTransparentTexture = MeshData.DefaultUseTransparentTexture;
				meshData.useDoubleFace = MeshData.DefaultUseDoubleFace;
			}
		}

		public override bool CanCombineWithBlock (byte extendId, Block block)
		{
			return false;
		}

		protected virtual void CalculatorDirection(Chunk chunk,int x,int y,int z,MeshData meshData,byte extendId,Direction direction)
		{
			switch(direction)
			{
			case Direction.back:
				FaceDataBack(chunk,x,y,z,meshData,extendId);
				break;
			case Direction.front:
				FaceDataFront(chunk,x,y,z,meshData,extendId);
				break;
			case Direction.up:
				FaceDataUp(chunk,x,y,z,meshData,extendId);
				break;
			case Direction.down:
				FaceDataDown(chunk,x,y,z,meshData,extendId);
				break;
			case Direction.left:
				FaceDataLeft(chunk,x,y,z,meshData,extendId);
				break;
			default:
				FaceDataRight(chunk,x,y,z,meshData,extendId);
				break;
			}
		}

		public override MeshColliderType GetMeshColliderType (byte extendId)
		{
			if(extendId == 0)return MeshColliderType.supportCollider;
			else return MeshColliderType.none;
		}

		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.Part;
		}

		public override int LightDamp (byte extendId)
		{
			return 1;
		}

		public override Rect GetUVRect(byte extendId,Direction direction)
		{
			Rect uvRect = new Rect(0,0,0,0);
			if(WorldTextureProvider.IsCanUseProvider)
			{
				uvRect = WorldTextureProvider.Instance.GetBlockTextureRect(BlockType,0,direction);
			}
			return uvRect;
		}

		private float GetAverageHeight(Block a,Block b,Block c,Block d)
		{
			int index = 0;
			float height = 0;
			//只要有一个顶点是静态水，那么当前高度就为1
			if(a.BlockType == BlockType.StillWater || b.BlockType == BlockType.StillWater ||
			   c.BlockType == BlockType.StillWater || d.BlockType == BlockType.StillWater)
				return 1f;
			if(a.BlockType == BlockType.FlowingWater)
			{
				if(a.ExtendId > waterExtendLevel)return 1f;
				index++;
				height += GetHeight(a.ExtendId);
			}

			if(b.BlockType == BlockType.FlowingWater)
			{
				if(b.ExtendId > waterExtendLevel)return 1f;
				index++;
				height += GetHeight(b.ExtendId);
			}

			if(c.BlockType == BlockType.FlowingWater)
			{
				if(c.ExtendId > waterExtendLevel)return 1f;
				index++;
				height += GetHeight(c.ExtendId);
			}

			if(d.BlockType == BlockType.FlowingWater)
			{
				if(d.ExtendId > waterExtendLevel)return 1f;
				index++;
				height += GetHeight(d.ExtendId);
			}
			return height / index;
		}

		private float GetHeight(byte extendId)
		{
			if(extendId > waterExtendLevel)
			{
				return 1f;
			}
			return 1f - extendId / (float)waterExtendLevel;
		}

		public virtual MeshData FaceDataUp
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{
			Block centerBlock = chunk.GetBlock(x,y,z);
			Block leftBlock = chunk.GetBlock(x - 1,y,z);
			Block rightBlock = chunk.GetBlock(x + 1,y,z);
			Block frontBlock = chunk.GetBlock(x,y,z + 1);
			Block backBlock = chunk.GetBlock(x,y,z-1);
			Block leftFront =chunk.GetBlock(x - 1,y,z + 1);
			Block leftBack = chunk.GetBlock(x - 1,y,z - 1);
			Block rightFront = chunk.GetBlock(x + 1,y,z + 1);
			Block rightBack = chunk.GetBlock(x + 1,y,z - 1);

			float leftFrontHeight = GetAverageHeight(leftBlock,centerBlock,leftFront,frontBlock);
			float rightFrontHeight = GetAverageHeight(rightBlock,centerBlock,rightFront,frontBlock);
			float rightBackHeight = GetAverageHeight(rightBlock,centerBlock,rightBack,backBlock);
			float leftBackHeight = GetAverageHeight(leftBlock,centerBlock,leftBack,backBlock);

//			//计算流体流动方向
//			float dicX = 2 * (leftBackHeight + leftFrontHeight - rightBackHeight - rightFrontHeight);
//			float dicZ = 2 * (leftBackHeight + rightBackHeight - leftFrontHeight - rightFrontHeight);
//			Vector2 dic = new Vector2(dicX,dicZ);
//			dic.Normalize();
//			if(dic.x == 0 && dic.y == 0)dic.x = 1;
//			float sinDic = dic.y;
//			float cosDic = dic.x;

			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightBackHeight,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftBackHeight,z));
			
			meshData.AddQuadTriangles();
			
//			AddSpecialUV(meshData,extendId,Direction.up);
			Vector2 dic = GetRotate(leftFrontHeight,rightFrontHeight,rightBackHeight,leftBackHeight);
			AddUVRotate(meshData,extendId,Direction.up,dic.y,dic.x);

			float speed = dic.magnitude == 0 ? 0 : 0.5f;
			AddSpecialColorAndSpeed(chunk,x,y,z,meshData,speed);
			
			return meshData;
		}

		protected void AddSpecialColorAndSpeed (Chunk chunk, int x, int y, int z, MeshData meshData,float speed = 0.5f)
		{
			int sunLight = chunk.GetSunLight(x,y,z);
			int blockLight = chunk.GetBlockLight(x,y,z); 
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],speed,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],speed,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],speed,2));
			meshData.AddColor(MeshBaseDataCache.Instance.GetColor(LightConst.lightColor[sunLight],LightConst.lightColor[blockLight],speed,2));
		}

		private Vector2 GetRotate(float leftFrontHeight,float rightFrontHeight,float rightBackHeight,float leftBackHeight)
		{
			//计算流体流动方向
			float dicX = 2 * (leftBackHeight + leftFrontHeight - rightBackHeight - rightFrontHeight);
			float dicZ = 2 * (leftBackHeight + rightBackHeight - leftFrontHeight - rightFrontHeight);
			Vector2 dic = new Vector2(dicX,dicZ);
			dic.Normalize();
			return dic;

		}

		private void AddUVRotate(MeshData meshData,byte extendId,Direction direction,float sinDic,float cosDic)
		{
			Direction resultDirection = GetRealDirection(extendId,Direction.up);
			
			Rect uvRect = GetUVRect(extendId,resultDirection);
			Vector2 center = uvRect.center;
			
			float scale = 0;
			meshData.AddUV(Rotate(center,MeshBaseDataCache.Instance.GetVector2(uvRect.x + scale,uvRect.y + scale),sinDic,cosDic));
			meshData.AddUV(Rotate(center,MeshBaseDataCache.Instance.GetVector2(uvRect.x + uvRect.width - scale,uvRect.y + scale),sinDic,cosDic));
			meshData.AddUV(Rotate(center,MeshBaseDataCache.Instance.GetVector2(uvRect.x + uvRect.width - scale,uvRect.y + uvRect.height - scale),sinDic,cosDic));
			meshData.AddUV(Rotate(center,MeshBaseDataCache.Instance.GetVector2(uvRect.x + scale, uvRect.y + uvRect.height - scale),sinDic,cosDic));
		}

		private Vector2 Rotate(Vector2 center,Vector2 orgin,float sin,float cos)
		{
			if(sin == 0 && cos == 0)return orgin;
			float x = orgin.x - center.x;
			float y = orgin.y - center.y;
			orgin.x = x * cos - y * sin + center.x;
			orgin.y = x * sin + y * cos + center.y;
			return orgin;
		}

		public virtual MeshData FaceDataDown
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y, z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z + 1f));
			
			meshData.AddQuadTriangles();

			AddSpecialUV(meshData,extendId,Direction.down);
			AddSpecialColor(chunk,x,y - 1,z,meshData);
			return meshData;
		}
		
		
		public virtual MeshData FaceDataBack
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{

			Block centerBlock = chunk.GetBlock(x,y,z);
			Block leftBlock = chunk.GetBlock(x - 1,y,z);
			Block rightBlock = chunk.GetBlock(x + 1,y,z);
			Block backBlock = chunk.GetBlock(x,y,z-1);
			Block leftBack = chunk.GetBlock(x - 1,y,z - 1);
			Block rightBack = chunk.GetBlock(x + 1,y,z - 1);
			
			float rightBackHeight = GetAverageHeight(leftBlock,centerBlock,backBlock,leftBack);
			float leftBackHeight = GetAverageHeight(rightBlock,centerBlock,backBlock,rightBack);
			float leftFrontHeight = 0;
			float rightFrontHeight = 0;

			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + rightBackHeight,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + leftBackHeight,z));
			
			meshData.AddQuadTriangles();
			
//			AddSpecialUV(meshData,extendId,Direction.back);
			Vector2 dic = GetRotate(leftFrontHeight,rightFrontHeight,rightBackHeight,leftBackHeight);
			AddUVRotate(meshData,extendId,Direction.back,dic.y,dic.x);

			AddSpecialColorAndSpeed(chunk,x,y,z - 1,meshData,2);
			
			return meshData;
		}
		
		public virtual MeshData FaceDataLeft
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{

			Block centerBlock = chunk.GetBlock(x,y,z);
			Block leftBlock = chunk.GetBlock(x - 1,y,z);
			Block frontBlock = chunk.GetBlock(x,y,z + 1);
			Block backBlock = chunk.GetBlock(x,y,z-1);
			Block leftFront =chunk.GetBlock(x - 1,y,z + 1);
			Block leftBack = chunk.GetBlock(x - 1,y,z - 1);

			
			float rightBackHeight = GetAverageHeight(leftBlock,centerBlock,leftFront,frontBlock);
			float leftBackHeight = GetAverageHeight(leftBlock,centerBlock,leftBack,backBlock);
			float rightFrontHeight = 0;
			float leftFrontHeight = 0;

			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftFrontHeight,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + rightFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + rightBackHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftBackHeight,z));
			
			meshData.AddQuadTriangles();
			
//			AddSpecialUV(meshData,extendId,Direction.left);
			Vector2 dic = GetRotate(leftFrontHeight,rightFrontHeight,rightBackHeight,leftBackHeight);
			AddUVRotate(meshData,extendId,Direction.left,dic.y,dic.x);

			AddSpecialColorAndSpeed(chunk,x - 1,y,z,meshData,2);
			
			return meshData;
		}
		
		public virtual MeshData FaceDataRight
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{

			Block centerBlock = chunk.GetBlock(x,y,z);
			Block rightBlock = chunk.GetBlock(x + 1,y,z);
			Block frontBlock = chunk.GetBlock(x,y,z + 1);
			Block backBlock = chunk.GetBlock(x,y,z-1);
			Block rightFront = chunk.GetBlock(x + 1,y,z + 1);
			Block rightBack = chunk.GetBlock(x + 1,y,z - 1);

			
			float rightBackHeight = GetAverageHeight(rightBlock,centerBlock,rightBack,backBlock);
			float leftBackHeight = GetAverageHeight(rightBlock,centerBlock,rightFront,frontBlock);
			float leftFrontHeight = 0;
			float rightFrontHeight = 0;

			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + leftFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightFrontHeight,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightBackHeight,z));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + leftBackHeight,z + 1f));
			
			meshData.AddQuadTriangles();
			
//			AddSpecialUV(meshData,extendId,Direction.right);
			Vector2 dic = GetRotate(leftFrontHeight,rightFrontHeight,rightBackHeight,leftBackHeight);
			AddUVRotate(meshData,extendId,Direction.right,dic.y,dic.x);

			AddSpecialColorAndSpeed(chunk,x + 1,y,z,meshData,2);
			
			return meshData;
		}
		
		public virtual MeshData FaceDataFront
			(Chunk chunk, int x, int y, int z, MeshData meshData,byte extendId)
		{

			Block centerBlock = chunk.GetBlock(x,y,z);
			Block leftBlock = chunk.GetBlock(x - 1,y,z);
			Block rightBlock = chunk.GetBlock(x + 1,y,z);
			Block frontBlock = chunk.GetBlock(x,y,z + 1);
			Block leftFront =chunk.GetBlock(x - 1,y,z + 1);
			Block rightFront = chunk.GetBlock(x + 1,y,z + 1);
			
			float rightBackHeight = GetAverageHeight(frontBlock,centerBlock,rightFront,rightBlock);
			float leftBackHeight = GetAverageHeight(frontBlock,centerBlock,leftFront,leftBlock);
			float leftFrontHeight = 0;
			float rightFrontHeight = 0;

			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightFrontHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x + 1f,y + rightBackHeight,z + 1f));
			meshData.AddVertice(MeshBaseDataCache.Instance.GetVector3(x,y + leftBackHeight,z + 1f));
			
			meshData.AddQuadTriangles();
			
//			AddSpecialUV(meshData,extendId,Direction.front);
			Vector2 dic = GetRotate(leftFrontHeight,rightFrontHeight,rightBackHeight,leftBackHeight);
			AddUVRotate(meshData,extendId,Direction.front,dic.y,dic.x);

			AddSpecialColorAndSpeed(chunk,x,y,z + 1,meshData,2);
			
			return meshData;
		}

//		protected override void AddSpecialColor (Chunk chunk, int x, int y, int z, MeshData meshData)
//		{
//			int sunLight = chunk.GetSunLight(x,y,z,true);
//			int blockLight = chunk.GetBlockLight(x,y,z,true); 
//			Color color = new Color(lightColor[sunLight],lightColor[blockLight],0,2);
//			meshData.AddColor(color);
//			meshData.AddColor(color);
//			meshData.AddColor(color);
//			meshData.AddColor(color);
//		}

		#endregion
		
		public BAC_FlowingWater ()
		{

		}

		public bool IsCanChangeToStillSourceWater(Block left,Block right,Block front,Block back)
		{
			int i = 0;
			if(CheckIsWaterSrouce(left))
			{
				i++;
			}
			if(CheckIsWaterSrouce(right))
			{
				i++;
			}
			if(CheckIsWaterSrouce(front))
			{
				i++;
			}
			if(CheckIsWaterSrouce(back))
			{
				i++;
			}
			if(i > 1)return true;
			return false;
		}

		private bool IsAroundNoFindWater(Block left,Block right,Block front,Block back,Block up,Block down)
		{
			if(!CheckIsWater(left) && !CheckIsWater(right) && !CheckIsWater(front) && !CheckIsWater(back) &&!CheckIsWater(up) && !CheckIsWater(down))
			{
				return false;
			}
			return true;
		}

		private bool CheckIsWaterSrouce(Block b)
		{
			if(b.BlockType == BlockType.StillWater || (b.BlockType == BlockType.FlowingWater && b.ExtendId == 0))
			{
				return true;
			}
			return false;
		}

		private bool CheckIsWater(Block b)
		{
			if(b.BlockType == BlockType.StillWater || b.BlockType == BlockType.FlowingWater)
			{
				return true;
			}
			return false;
		}

	}
}

