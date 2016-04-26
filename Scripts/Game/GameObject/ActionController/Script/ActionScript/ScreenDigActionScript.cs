using System;
using UnityEngine;
namespace MTB
{
	public class ScreenDigActionScript : BaseActionScript
	{
		private Block air = new Block(BlockType.Air);

		private GOPlayerController _playerController;

		private int oppoMaskLayer;
		private float distance;

		public ScreenDigActionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
			_playerController = gameObjectController as GOPlayerController;
		}

		public override void SetParam (System.Collections.Generic.Dictionary<string, string> param)
		{
			string oppoMaskLayerStr = "";
			param.TryGetValue("oppoMaskLayer",out oppoMaskLayerStr);
			oppoMaskLayer = ~0;
			if(oppoMaskLayerStr != "")
			{
				string[] layerNames = oppoMaskLayerStr.Split('|');
				oppoMaskLayer = ~LayerMask.GetMask(layerNames);
			}
			
			distance = Convert.ToSingle(param["distance"]);
			
//			rayDistance = param.ContainsKey("rayDistance") ? Convert.ToSingle(param["rayDistance"]) : 2000;
		}

		public override void ActionIn ()
		{
			float screenX = _playerController.playerInputState.X;
			float screenY = _playerController.playerInputState.Y;
			BlockMaskController.Instance.Do(screenX,screenY,_playerController.transform.position,distance);
			MineController.Instance.StartMine();
			_playerController.goActionController.On_AnimatorEvent += HandleOn_AnimatorEvent;
		}
		
		void HandleOn_AnimatorEvent (UnityEngine.Object value)
		{
			float screenX = _playerController.playerInputState.X;
			float screenY = _playerController.playerInputState.Y;
			RaycastHit hit;
			if(Terrain.RayToWorld(screenX,screenY,_playerController.transform.position,
			                      distance,out hit,oppoMaskLayer))
			{
				WorldPos pos = Terrain.GetWorldPos(hit,false);
				if(hit.collider.GetComponentInParent<ChunkObj>() == null)return;
				Block hitBlock = World.world.GetBlock(pos.x,pos.y,pos.z);
				if(hitBlock.BlockType == BlockType.Air)return;
				int handMaterialId = _playerController.playerAttribute.handMaterialId;
				if(MineController.Instance.Mine(handMaterialId,pos,hitBlock))
				{
					BlockExplodeController.Instance.Explode(pos,hitBlock,hit.normal);
					Terrain.SetBlock(pos,air);
					DropController.Instance.Drop(handMaterialId,pos,hitBlock);
				}
			}
		}

//		public override void ActionDoing ()
//		{
//			float screenX = _playerController.playerInputState.X;
//			float screenY = _playerController.playerInputState.Y;
//			BlockMaskController.Instance.Do(screenX,screenY,_playerController.transform.position,distance);
//		}
		
		public override void ActionOut ()
		{
			MineController.Instance.StopMine();
			BlockMaskController.Instance.StopDo();
			_playerController.goActionController.On_AnimatorEvent -= HandleOn_AnimatorEvent;
		}
	}
}

