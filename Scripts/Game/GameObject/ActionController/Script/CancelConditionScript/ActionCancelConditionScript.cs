using System;
using System.Collections.Generic;
namespace MTB
{
	public class ActionCancelConditionScript : BaseCancelConditionScript
	{
		private List<int> cancelActionIds;
		public ActionCancelConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		public override void SetParam (Dictionary<string, string> param)
		{
			cancelActionIds = new List<int>();
			string[] cancelActionIdStrs = param["cancelActionIds"].Split(',');
			for (int i = 0; i < cancelActionIdStrs.Length; i++) {
				cancelActionIds.Add(Convert.ToInt32(cancelActionIdStrs[i]));
			}
		}

		public override bool MeetCondition ()
		{
//			if(cancelActionIds.Contains(_gameObjectController.goActionController.curAction.actionData.id))return true;
			return false;
		}
	}
}

