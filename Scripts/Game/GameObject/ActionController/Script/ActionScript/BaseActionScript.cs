using System;
namespace MTB
{
	public class BaseActionScript : BaseScript
	{
		public BaseActionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		public virtual void SetRunTimeActionParam(ActionParam param){}

		public virtual void ActionIn(){}
		public virtual void ActionOut(){}
		public virtual void ActionDoing(){}
	}
}

