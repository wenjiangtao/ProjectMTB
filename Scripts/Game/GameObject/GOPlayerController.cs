using System;
using UnityEngine;
namespace MTB
{
	[RequireComponent(typeof(PlayerMovableController))]
	[RequireComponent(typeof(PlayerAttributes))]
	public class GOPlayerController : GameObjectController
	{
		public PlayerInputState playerInputState{get{return gameObjectInputState as PlayerInputState;}}
		public PlayerAttributes playerAttribute{get{return baseAttribute as PlayerAttributes;}}
		public bool isFreeMode{get;private set;}

//		protected override void Start ()
//		{
//			ResetPos();
//			InitState();
//			_goActionController = new GOPlayerActionController(this);
//			_goActionController.BindAnimatorController(objectView.animatorController);
//			IsRead = true;
//			isFreeMode = false;
//		}

		protected override void InitState ()
		{
			_gameObjectState = new PlayerState(this);
			_gameObjectInputState = new PlayerInputState(this);
		}

		protected override void InitOtherInfo ()
		{
			ResetPos();
			isFreeMode = false;
		}

		protected override void InitActionController ()
		{
			_goActionController = new GOPlayerActionController(this);
			_goActionController.BindAnimatorController(objectView.animatorController);
		}

		protected override void ResetPos()
		{
			WorldPos pos = Terrain.GetWorldPos(transform.position);
			int height = WorldConfig.Instance.heightCap;
			for (int y = height - 1; y >= 0; y--)
			{
				if (World.world.GetBlock(pos.x, y + World.MinHeight, pos.z).BlockType != BlockType.Air)
				{
					height = y + World.MinHeight;
					break;
				}
			}
			this.gameObject.transform.position = new Vector3(transform.position.x, (float)height + 1, transform.position.z);
		}

		public virtual void ChangeToFree(bool isFree)
		{
			if(isFree && _movableController.CurMovableCondition.movableConditionType != MovableConditionType.Free)
			{
				_movableController.ChangeMovableCondition(MovableConditionType.Free);
				isFreeMode = true;
			}
			if(!isFree && _movableController.CurMovableCondition.movableConditionType == MovableConditionType.Free)
			{
				_movableController.ChangeMovableCondition(MovableConditionType.Ground);
				isFreeMode = false;
			}
		}

		protected override void UpdateMovableCondition ()
		{
			if(_movableController.CurMovableCondition.movableConditionType != MovableConditionType.Free)
			{
				base.UpdateMovableCondition ();
			}
		}

		public override void ChangeObjectView (ObjectView objectView)
		{
			base.ChangeObjectView (objectView);
			_goActionController.BindAnimatorController(this.objectView.animatorController);
			this.objectView.handMountPointController.ChangeHandObj(playerAttribute.handMaterialId,CameraManager.Instance.CurCamera.CameraType == MTBCameraType.First ? true: false);
		}

		public void ChangeHandleMaterial(int materialId)
		{
			playerAttribute.handMaterialId = materialId;
			objectView.handMountPointController.ChangeHandObj(playerAttribute.handMaterialId,CameraManager.Instance.CurCamera.CameraType == MTBCameraType.First ? true: false);
		}
	}

	public class PlayerState : GameObjectState{
		public GOPlayerController controller{get{return _controller as GOPlayerController;}}
		public PlayerState(GameObjectController controller)
			:base(controller)
		{
		}
	}

	public class PlayerInputState : GameObjectInputState{
		public delegate void InputActionChangeHandler(InputType inputType,int inputValue,int oldInputValue);
		public event InputActionChangeHandler On_InputActionChange;
		private InputActionType _inputActionType;
		public InputActionType inputActionType{
			get{return _inputActionType;}
			set{
				if(_inputActionType != value)
				{
					InputActionType old = _inputActionType;
					_inputActionType = value;
					UpdateAction(InputType.ScreenTouch,(int)_inputActionType,(int)old);
				}
			}
		}

		public float X{get;set;}
		public float Y{get;set;}

		public PlayerInputState(GameObjectController controller)
			:base(controller)
		{
			_inputActionType = InputActionType.Null;
		}

		public override void UpdateAction (InputType inputType, int inputValue, int oldValue)
		{
			if(On_InputActionChange != null)
			{
				On_InputActionChange(inputType,inputValue,oldValue);
			}
		}
	}
}

