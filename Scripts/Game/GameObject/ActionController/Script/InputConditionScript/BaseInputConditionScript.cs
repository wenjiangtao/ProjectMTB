using System;
namespace MTB
{
	public class BaseInputConditionScript : BaseScript
	{
		public BaseInputConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}
		public virtual bool MeetCondition(){return true;}
	}
}

