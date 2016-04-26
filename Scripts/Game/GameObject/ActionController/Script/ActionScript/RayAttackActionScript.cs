using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class RayAttackActionScript : BaseActionScript
	{
		private int maskLayer;
		private Vector3 startOffset;
		private float distance;
		public RayAttackActionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{

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
			startOffset = Vector3.zero;
			string startOffsetStr = "";
			param.TryGetValue("startOffset",out startOffsetStr);
			if(startOffsetStr != "")
			{
				string[] startOffsets = startOffsetStr.Split(',');
				float x = Convert.ToSingle(startOffsets[0]);
				float y = Convert.ToSingle(startOffsets[1]);
				float z = Convert.ToSingle(startOffsets[2]);
				startOffset = new Vector3(x,y,z);
			}

			distance = Convert.ToSingle(param["distance"]);
		}

		public override void ActionIn ()
		{
			RaycastHit hit;
			
			var startPosition =_gameObjectController.transform.position + startOffset;
			var direction = _gameObjectController.transform.forward;
			if (Physics.Raycast(startPosition, direction, out hit, distance, maskLayer))
			{
				GameObjectController controller = hit.collider.GetComponent<GameObjectController>();
				if(controller == null)return;
				controller.BeAttack(new BeAttackParam(direction));
			}
		}
	}
}

