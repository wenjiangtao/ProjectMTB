using System;
using UnityEngine;
namespace MTB
{
	public class ScreenRayAttackActionScript : BaseActionScript
	{
		private int maskLayer;
		private float distance;
		private float rayDistance;
		private GOPlayerController _playerController;
		public ScreenRayAttackActionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
			_playerController = gameObjectController as GOPlayerController;
		}

		public override void SetParam (System.Collections.Generic.Dictionary<string, string> param)
		{
			string maskLayerStr = "";
			param.TryGetValue("maskLayer",out maskLayerStr);
			maskLayer = 0;
			if(maskLayerStr != "")
			{
				string[] layerNames = maskLayerStr.Split('|');
				maskLayer = LayerMask.GetMask(layerNames);
			}

			distance = Convert.ToSingle(param["distance"]);

			rayDistance = param.ContainsKey("rayDistance") ? Convert.ToSingle(param["rayDistance"]) : 2000;
		}

		public override void ActionIn ()
		{
			float screenX = _playerController.playerInputState.X;
			float screenY = _playerController.playerInputState.Y;
			RaycastHit hit;
			Ray ray = CameraManager.Instance.CurCamera.followCamera.ScreenPointToRay(new Vector3(screenX, screenY, 0));
			
			bool IsHit = Physics.Raycast(ray.origin, ray.direction, out hit, rayDistance, maskLayer);
			if (IsHit && Vector3.Distance(_playerController.transform.position, hit.point)
			    < distance)
			{
				GameObjectController controller = hit.collider.GetComponent<GameObjectController>();
				if(controller == null)return;
				controller.BeAttack(new BeAttackParam(ray.direction));
			}
		}
	}
}

