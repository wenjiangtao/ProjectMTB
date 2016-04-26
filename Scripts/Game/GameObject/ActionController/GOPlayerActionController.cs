using System;
namespace MTB
{
	public class GOPlayerActionController : GOActionController
	{
		public GOPlayerActionController (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		protected override GameObjectActionData GetActionDatas (int objectId)
		{
			return ActionDataManager.Instance.PlayerActionConfig.GetGameObjectActionDataById(objectId);
		}
	}
}

