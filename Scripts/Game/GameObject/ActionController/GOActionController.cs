using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class GOActionController
	{

		public delegate void OnAnimatorEvent(UnityEngine.Object value);
		public event OnAnimatorEvent On_AnimatorEvent;

		private AnimatorController _curAnimatorController;

		private GameObjectController _gameObjectController;
		public GameObjectController gameObjectController{get{return _gameObjectController;}}

		private Dictionary<int,GOAction> _actionMap;
		private Dictionary<int,GOAction> _nameHashActionMap;
		private GameObjectActionData _gameObjectActionData;
		private GOAction _curAction;
		public GOAction curAction{get{return _curAction;}}
		public bool isNetObj{get;private set;}

		public GOActionController(GameObjectController gameObjectController)
		{
			_gameObjectController = gameObjectController;
			isNetObj = gameObjectController.baseAttribute.isNetObj;
			InitActionData();
			InitAction();
		}
		//初始化当前对象的action
		public void InitAction()
		{
			_actionMap = new Dictionary<int, GOAction>();
			List<ActionData> list = _gameObjectActionData.GetListActionData();
			for (int i = 0; i < list.Count; i++) {
				GOAction action = new GOAction(_gameObjectController,list[i],isNetObj);
				_actionMap.Add(action.actionData.id,action);
			}
		}

		private void InitActionData()
		{
			int objectId = _gameObjectController.baseAttribute.objectId;
			_gameObjectActionData = GetActionDatas(objectId);
		}

		protected virtual GameObjectActionData GetActionDatas(int objectId)
		{
			return null;
		}

		public virtual void ChangeNetObj(bool isNetObj)
		{
			if(this.isNetObj != isNetObj)
			{
				this.isNetObj = isNetObj;
				InitAction();
				//从网络对象切换到当前对象的话，做默认动作
				GOAction defaultAction = GetAction(_gameObjectActionData.defaultId);
				ReallyDoAction(defaultAction);
			}
		}

		public void BindAnimatorController(AnimatorController animatorController)
		{
			if(_curAnimatorController != null)
			{
				_curAnimatorController.On_ActionStateEnter -= HandleOn_ActionStateEnter;
				_curAnimatorController.On_AnimatorEvent -= HandleOn_AnimatorEvent;
				_curAnimatorController.Dispose();
			}
			_curAnimatorController = animatorController;
			UpdateNameHashActionMap();
			_curAnimatorController.On_ActionStateEnter += HandleOn_ActionStateEnter;
			_curAnimatorController.On_AnimatorEvent += HandleOn_AnimatorEvent;
			//强制做默认动作
			ActionData actionData = _gameObjectActionData.GetActionData(_gameObjectActionData.defaultId);
			_curAnimatorController.animator.Play(actionData.animName,actionData.animLayer);
			GOAction defaultAction = GetAction(_gameObjectActionData.defaultId);
			ReallyDoAction(defaultAction);
		}

		private void UpdateNameHashActionMap()
		{
			_nameHashActionMap = new Dictionary<int, GOAction>();
			foreach (GOAction item in _actionMap.Values) {
				string layerName = _curAnimatorController.GetLayerName(item.actionData.animLayer);
				int nameHash = Animator.StringToHash(layerName + "." + item.actionData.animName);
				_nameHashActionMap.Add(nameHash,item);
			}
		}

		void HandleOn_AnimatorEvent (UnityEngine.Object value)
		{
			if(On_AnimatorEvent != null)
			{
				On_AnimatorEvent(value);
			}
		}

		public virtual void ChangeActionSpeed(float speed)
		{
			_curAnimatorController.ChangeSpeed(speed);
		}
		
		//监听动画事件，如果动画发生改变，那么当前行为改变
		protected void HandleOn_ActionStateEnter (StateInfo info)
		{
			GOAction action = GetActionByAnimNameHash(info.fullNameHash);
			ReallyDoAction(action);
		}
		
		public virtual bool DoAction(int actionId,ActionParam param = null)
		{
			GOAction action = GetAction(actionId);
			if(!action.CanDoAction())return false;
			if(!_curAction.CanCancelAction())return false;
			_curAnimatorController.DoAction(action.actionData.animName);
			return true;
		}
		
		protected void ReallyDoAction(GOAction action)
		{
			if(_curAction != null)
			{
				_curAction.ActionOut();
			}
			_curAction = action;
			_curAction.ActionIn();
		}

		public GOAction GetAction(int actionId)
		{
			GOAction action;
			_actionMap.TryGetValue(actionId,out action);
			return action; 
		}

		public GOAction GetActionByAnimNameHash(int nameHash)
		{
			GOAction action;
			_nameHashActionMap.TryGetValue(nameHash,out action);
			return action; 
		}

		public void Update()
		{
			if(_curAction != null)
			{
				_curAction.ActionDoing();
			}
		}
	}
}

