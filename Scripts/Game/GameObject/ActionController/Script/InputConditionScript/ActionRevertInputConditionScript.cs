using System;
using System.Collections.Generic;
namespace MTB
{
	public class ActionRevertInputConditionScript : BaseInputConditionScript
	{
		private List<int> revertActionIds;
		public ActionRevertInputConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		public override void SetParam (System.Collections.Generic.Dictionary<string, string> param)
		{
			revertActionIds = new List<int>();
			string[] revertActionIdStrs = param["revertActionIds"].Split(',');
			for (int i = 0; i < revertActionIdStrs.Length; i++) {
				revertActionIds.Add(Convert.ToInt32(revertActionIdStrs[i]));
			}
		}

		public override bool MeetCondition ()
		{
			if(revertActionIds.Contains(_gameObjectController.goActionController.curAction.actionData.id))
			{
				return true;
			}
			return false;
		}
	}
}

