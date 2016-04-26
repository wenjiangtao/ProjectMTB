using System;
using System.Collections.Generic;
namespace MTB
{
	public class BaseScript
	{
		protected GameObjectController _gameObjectController;
		public BaseScript (GameObjectController gameObjectController)
		{
			_gameObjectController = gameObjectController;
		}

		public virtual void SetParam(Dictionary<string,string> param){}

		public virtual void Dispose(){_gameObjectController = null;}
	}
}

