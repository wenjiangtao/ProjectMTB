using System;
using UnityEngine;
namespace MTB
{
    public class AttackInputConditionScript : ScreenRayInputConditionScript
    {
        public AttackInputConditionScript(GameObjectController gameObjectController)
            : base(gameObjectController)
        {
        }

        protected override bool HitDetailCondition(UnityEngine.RaycastHit hit)
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.GetComponent<MonsterAttributes>() == null)
                return false;
            MonsterBreedData data = DataManagerM.Instance.getMonsterDataManager().getBreedDate(obj);
            if (data.breedItem != _playerController.playerAttribute.handMaterialId) return true;
            return false;
        }
    }
}

