using System;
namespace MTB
{
    public class GONpcActionController : GOActionController
    {
        public GONpcActionController(GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		protected override GameObjectActionData GetActionDatas (int objectId)
		{
			return ActionDataManager.Instance.NpcActionConfig.GetGameObjectActionDataById(objectId);
		}
    }
}
