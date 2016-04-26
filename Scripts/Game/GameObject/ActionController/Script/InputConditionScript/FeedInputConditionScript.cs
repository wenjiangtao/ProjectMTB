using System;
using UnityEngine;
namespace MTB
{
	public class FeedInputConditionScript : ScreenRayInputConditionScript
	{
		public FeedInputConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		protected override bool HitDetailCondition (UnityEngine.RaycastHit hit)
		{
			GameObject obj = hit.collider.gameObject;
			MonsterBreedData data = DataManagerM.Instance.getMonsterDataManager().getBreedDate(obj);
			if(data.breedItem == _playerController.playerAttribute.handMaterialId)return true;
			return false;
		}
	}
}

