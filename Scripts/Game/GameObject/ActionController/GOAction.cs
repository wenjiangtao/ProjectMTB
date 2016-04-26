using System;
using System.Collections.Generic;
namespace MTB
{
	public class GOAction
	{
		public ActionData actionData{get;private set;}
		private GameObjectController _controller;

		private BaseActionScript _baseActionScript;
		private List<BaseDoConditionScript> _baseDoCondtionScripts;
		private List<BaseCancelConditionScript> _baseCancelConditionScripts;

		public GOAction (GameObjectController gameObjectController,ActionData actionData,bool isNet = false)
		{
			_controller = gameObjectController;
			this.actionData = actionData;
			Init(isNet);
		}

		private void Init(bool isNet)
		{
			string actionName = isNet ? actionData.action.netName : actionData.action.name;
			_baseActionScript = ScriptFactory.GetActionScript(actionName,_controller);
			_baseActionScript.SetParam(actionData.action.dataParam);

			_baseDoCondtionScripts = new List<BaseDoConditionScript>();
			for (int i = 0; i < actionData.doConditions.Count; i++) {
				BaseDoConditionScript doConditionScript = ScriptFactory.GetDoScript(actionData.doConditions[i].name,_controller);
				doConditionScript.SetParam(actionData.doConditions[i].dataParam);
				_baseDoCondtionScripts.Add(doConditionScript);
			}

			_baseCancelConditionScripts = new List<BaseCancelConditionScript>();
			for (int i = 0; i < actionData.cancelConditions.Count; i++) {
				BaseCancelConditionScript cancelConditionScript = ScriptFactory.GetCancelScript(actionData.cancelConditions[i].name,_controller);
				cancelConditionScript.SetParam(actionData.cancelConditions[i].dataParam);
				_baseCancelConditionScripts.Add(cancelConditionScript);
			}
		}

		public bool CanDoAction()
		{
			for (int i = 0; i < _baseDoCondtionScripts.Count; i++) {
				if(!_baseDoCondtionScripts[i].MeetCondition())return false;
			}
			return true;
		}
		public bool CanCancelAction()
		{
			for (int i = 0; i < _baseCancelConditionScripts.Count; i++) {
				if(!_baseCancelConditionScripts[i].MeetCondition())return false;
			}
			return true;
		}
		public void ActionIn(){_baseActionScript.ActionIn();}
		public void ActionOut(){_baseActionScript.ActionOut();}
		public void ActionDoing(){_baseActionScript.ActionDoing();}

		public void Dispose()
		{
			_baseActionScript.Dispose();
			for (int i = 0; i < _baseDoCondtionScripts.Count; i++) {
				_baseDoCondtionScripts[i].Dispose();
			}
			for (int i = 0; i < _baseCancelConditionScripts.Count; i++) {
				_baseCancelConditionScripts[i].Dispose();
			}
		}
	}
}

