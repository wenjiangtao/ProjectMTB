using System;
namespace MTB
{
	public class BaseDoConditionScript : BaseScript
	{
		public BaseDoConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		public virtual bool MeetCondition(){return true;}
	}
}

