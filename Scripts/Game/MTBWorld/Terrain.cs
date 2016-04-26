using System;
using UnityEngine;
namespace MTB
{
	public class Terrain 
	{
		public Terrain ()
		{
		}

		public static WorldPos GetWorldPos(Vector3 pos)
		{
//			WorldPos result = new WorldPos(
//				Mathf.RoundToInt(pos.x),
//				Mathf.RoundToInt(pos.y),
//				Mathf.RoundToInt(pos.z));
			WorldPos result = new WorldPos(
				Mathf.FloorToInt(pos.x),
				Mathf.FloorToInt(pos.y),
				Mathf.FloorToInt(pos.z));
			return result;
		}

		public static WorldPos GetChunkPos(int x,int y,int z)
		{
			int posX = Mathf.FloorToInt(x / (float)Chunk.chunkWidth)* Chunk.chunkWidth;
			int posY = Mathf.FloorToInt(y / (float)Chunk.chunkHeight) * Chunk.chunkHeight;
			int posZ = Mathf.FloorToInt(z / (float)Chunk.chunkDepth) * Chunk.chunkDepth;
			WorldPos pos = new WorldPos(posX,posY,posZ);
			return pos;
		}

		public static WorldPos GetChunkPos(Vector3 position)
		{
			WorldPos pos = GetWorldPos(position);
			return GetChunkPos(pos.x,pos.y,pos.z);
		}

		//adjacent为true是表示是选中对象相邻的块（方向为hit法向量方向）
		public static WorldPos GetWorldPos(RaycastHit hit,bool adjacent = false)
		{
			if(IsPosInBlock(hit.point))
			{
				Vector3 result = hit.point;
				if(adjacent)
				{
					result += hit.normal;
				}
				return GetWorldPos(result);
			}
			else
			{
				Vector3 pos = new Vector3(
						MoveToBlockRealPos(hit.point.x,hit.normal.x,adjacent),
						MoveToBlockRealPos(hit.point.y,hit.normal.y,adjacent),
						MoveToBlockRealPos(hit.point.z,hit.normal.z,adjacent)
					);
				return GetWorldPos(pos);
			}
		}

		private static bool IsPosInBlock(Vector3 pos)
		{
			if(!Mathf.Approximately(pos.x,Mathf.Round(pos.x))
			   && !Mathf.Approximately(pos.y,Mathf.Round(pos.y))
			   && !Mathf.Approximately(pos.z,Mathf.Round(pos.z)))
			{
				return true;
			}
			return false;
		}

		private static float MoveToBlockRealPos(float origin,float normal,bool adjacent = false)
		{
			//解决精度丢失问题（origin可能为xxx.49998f）
//			if(Mathf.Abs(origin - Mathf.FloorToInt(origin)) < 0.0001f)
//			{
			if(Mathf.Approximately(origin,Mathf.Round(origin)))
			{
				if(adjacent)
				{
					origin += normal / 2;
				}
				else
				{
					origin -= normal / 2;
				}
			}
//			}
			return Mathf.FloorToInt(origin);
		}
//		public static bool SetBlock(RaycastHit hit,Block block,bool adjacent = false)
//		{
//			if(hit.collider == null)
//			{
//				return false;
//			}
//			ChunkObj chunkObj = hit.collider.GetComponentInParent<ChunkObj>();
//			if(chunkObj == null)
//			{
//				return false;
//			}
//			WorldPos pos = GetWorldPos(hit,adjacent);
//			if(!CheckPosCanPlaced(pos))return false;
//			BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(block.BlockType);
//			dispatcher.SetBlock(chunkObj.chunk.world,pos.x,pos.y,pos.z,block);
//			return true;
//		}

		public static void SetBlock(WorldPos pos,Block block)
		{
			BlockDispatcher dispatcher = BlockDispatcherFactory.GetBlockDispatcher(block.BlockType);
			dispatcher.SetBlock(World.world,pos.x,pos.y,pos.z,block);
		}

		public static bool CheckPosCanPlaced(Transform obj,WorldPos pos)
		{
			CharacterController controller = obj.GetComponent<CharacterController>();
			Vector3 curPos = obj.position;
			WorldPos playerWorldPos = GetWorldPos(curPos);
			int up = Mathf.FloorToInt(curPos.y + controller.height);
			int down = Mathf.FloorToInt(curPos.y);
			int left = Mathf.FloorToInt(curPos.x - controller.radius);
			int right = Mathf.FloorToInt(curPos.x + controller.radius);
			int front = Mathf.FloorToInt(curPos.z + controller.radius);
			int back = Mathf.FloorToInt(curPos.z - controller.radius);
			if(pos.x >= left && pos.x <= right && pos.z >= back && pos.z <= front && pos.y >= down && pos.y <= up)
			{
				return false;
			}
			return true;
		}

		public static bool RayToWorld(float screenX,float screenY,Vector3 originPos,float maxDistance, out RaycastHit hit,int maskLayer)
		{
			Ray ray = CameraManager.Instance.CurCamera.followCamera.ScreenPointToRay(new Vector3(screenX, screenY, 0));
			bool IsHit = Physics.Raycast(ray.origin, ray.direction, out hit, 2000, maskLayer);
            if (IsHit && Vector3.Distance(originPos, hit.point) < maxDistance)
			{
				return true;
			}
			return false;
		}

		public static bool RayToWorld(float screenX,float screenY,Vector3 originPos,float maxDistance, out RaycastHit hit)
		{
			return RayToWorld(screenX,screenY,originPos,maxDistance,out hit,~0);
		}
	}
}

