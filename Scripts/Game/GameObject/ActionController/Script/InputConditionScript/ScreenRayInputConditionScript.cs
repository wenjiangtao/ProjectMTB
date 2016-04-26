using System;
using UnityEngine;
namespace MTB
{
	public class ScreenRayInputConditionScript : BaseInputConditionScript
	{
		private int oppoMaskLayer;
		private float distance;
		private int resultLayer;
		protected GOPlayerController _playerController;
		public ScreenRayInputConditionScript (GameObjectController gameObjectController)
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

			string resultLayerStr = "";
			param.TryGetValue("resultLayer",out resultLayerStr);
			resultLayer = 0;
			if(resultLayerStr != "")
			{
				string[] layerNames = resultLayerStr.Split('|');
				resultLayer = LayerMask.GetMask(layerNames);
			}

			distance = Convert.ToSingle(param["distance"]);
		}

		public override bool MeetCondition ()
		{
			float screenX = _playerController.playerInputState.X;
			float screenY = _playerController.playerInputState.Y;
			RaycastHit hit;
			if(Terrain.RayToWorld(screenX,screenY,_playerController.transform.position,
			                      distance,out hit,oppoMaskLayer))
			{
				if((resultLayer & (1 << hit.collider.gameObject.layer)) == 0)
				{
					return false;
				}
				else
				{
					return HitDetailCondition(hit);
				}
			}
			return false;
		}

		protected virtual bool HitDetailCondition(RaycastHit hit)
		{
			return true;
		}
	}
}

