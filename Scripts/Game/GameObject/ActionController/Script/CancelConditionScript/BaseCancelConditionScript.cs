using System;
namespace MTB
{
	public class BaseCancelConditionScript : BaseScript
	{
		public BaseCancelConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}
		public virtual bool MeetCondition(){return true;}
	}
}

