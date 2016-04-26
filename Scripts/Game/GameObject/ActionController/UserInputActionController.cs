using System;
using System.Collections.Generic;
namespace MTB
{
	public class UserInputActionController
	{
		private Dictionary<InputType,Dictionary<int,List<UserInputAction>>> _map;
		private GOPlayerController _controller;
		public UserInputActionController ()
		{
		}

		public void BinderGameObjectController(GOPlayerController controller)
		{
			if(_controller != null)
			{
				RemoveListener();
				DisposeScript();
			}
			_controller = controller;

			InitScript();
			AddListener();
		}

		private void InitScript()
		{
			_map =new Dictionary<InputType, Dictionary<int, List<UserInputAction>>>();
			List<InputActionData> list = InputActionDataManager.Instance.GetData(1).listActionData;
			for (int i = 0; i < list.Count; i++) {
				var inputAction = new UserInputAction(_controller,list[i]);
				InputType inputType = list[i].inputType;
				int inputValue = list[i].inputValue;
				if(!_map.ContainsKey(inputType))
				{
					_map.Add(inputType,new Dictionary<int, List<UserInputAction>>());
				}
				if(!_map[inputType].ContainsKey(inputValue))
				{
					_map[inputType].Add(inputValue,new List<UserInputAction>());
				}
				_map[inputType][inputValue].Add(inputAction);
			}
		}

		private void AddListener()
		{
			_controller.playerInputState.On_InputActionChange += HandleOn_InputActionChange;
		}

		void HandleOn_InputActionChange (InputType inputType, int inputValue, int oldInputValue)
		{
			List<UserInputAction> list = GetInputAction(inputType,inputValue);
			if(list == null)return;
			for (int i = 0; i < list.Count; i++) {
				if(list[i].InputMeetAction())
				{
					_controller.DoAction(list[i].inputActionData.actionId);
				}
			}
		}

		private void RemoveListener()
		{
			_controller.playerInputState.On_InputActionChange -= HandleOn_InputActionChange;
		}

		private List<UserInputAction> GetInputAction(InputType inputType,int inputValue)
		{
			if(_map.ContainsKey(inputType) && _map[inputType].ContainsKey(inputValue))
			{
				return _map[inputType][inputValue];
			}
			return null;
		}

		private void DisposeScript()
		{
			foreach (var item in _map) {
				foreach (var item1 in item.Value) {
					foreach (var item2 in item1.Value) {
						item2.Dispose();
					}
				}
			}
		}
	}
}

