using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class BD_FlowingWater : BlockDispatcher
	{
		public override void SetBlock (World world, int x, int y, int z, Block block,bool notify = true)
		{
			if(!IsHeightLimit(y))return;
			Chunk chunk = world.GetChunk(x,y,z);
			if(chunk == null)return;

			SetBlockAndNotify(world,x,y,z,block);
			world.CheckAndRecalculateMesh(x,y,z,block);

			LiquidSpreadSource spreadSource = LiquidSpreadSourceManager.GetSpreadSource();
			spreadSource.chunk = chunk;
			spreadSource.liquidType = block.BlockType;
//			spreadSource.AddRelationLiquidType(BlockType.StillWater);
			spreadSource.pos = new WorldPos(x - chunk.worldPos.x,y - chunk.worldPos.y,z - chunk.worldPos.z);
			spreadSource.curSpreadLevel = block.ExtendId > BAC_FlowingWater.waterExtendLevel ? 0 : (int)block.ExtendId;
			spreadSource.spreadMaxLevel = BAC_FlowingWater.waterExtendLevel;
			spreadSource.OnSpreadPlaneFinish += HandleOnSpreadPlaneFinish;
			spreadSource.StartSpread();
		}

		void HandleOnSpreadPlaneFinish (LiquidSpreadSource source)
		{
			source.OnSpreadPlaneFinish -= HandleOnSpreadPlaneFinish;
			List<WorldPos> list = source.GetNextSpreadSourceList();
			for (int i = 0; i < list.Count; i++) {
				BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(source.liquidType);
				dispatcher.SetBlock(source.chunk.world,list[i].x,list[i].y,list[i].z,new Block(source.liquidType,(byte)(source.curSpreadLevel + source.spreadMaxLevel)));
			}
			LiquidSpreadSourceManager.SaveSpreadSource(source);
		}

		public BD_FlowingWater ()
		{
		}
	}
}

