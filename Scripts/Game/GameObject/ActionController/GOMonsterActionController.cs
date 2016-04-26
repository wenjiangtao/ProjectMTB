using System;
namespace MTB
{
	public class GOMonsterActionController : GOActionController
	{
		public GOMonsterActionController (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		protected override GameObjectActionData GetActionDatas (int objectId)
		{
			return ActionDataManager.Instance.MonsterActionConfig.GetGameObjectActionDataById(objectId);
		}
	}
}

