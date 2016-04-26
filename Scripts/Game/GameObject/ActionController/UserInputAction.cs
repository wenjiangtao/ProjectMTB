using System;
using System.Collections.Generic;
namespace MTB
{
	public class UserInputAction
	{
		public InputActionData inputActionData{get;private set;}
		private GameObjectController _controller;

		private List<BaseInputConditionScript> _conditionScripts;
		public UserInputAction (GameObjectController controller,InputActionData inputActionData)
		{
			_controller = controller;
			this.inputActionData = inputActionData;
			Init();
		}

		private void Init()
		{
			_conditionScripts = new List<BaseInputConditionScript>();
			for (int i = 0; i < inputActionData.inputConditions.Count; i++) {
				BaseInputConditionScript inputActionConditionScript = ScriptFactory.GetInputConditionScript(inputActionData.inputConditions[i].name,_controller);
				inputActionConditionScript.SetParam(inputActionData.inputConditions[i].dataParam);
				_conditionScripts.Add(inputActionConditionScript);
			}
		}

		public bool InputMeetAction()
		{
			for (int i = 0; i < _conditionScripts.Count; i++) {
				if(!_conditionScripts[i].MeetCondition())return false;
			}
			return true;
		}

		public void Dispose()
		{
			for (int i = 0; i < _conditionScripts.Count; i++) {
				_conditionScripts[i].Dispose();
			}
		}
	}
}

