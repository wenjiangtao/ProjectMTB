using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	[RequireComponent(typeof(CharacterMotor))]
	abstract public class MovableController : MonoBehaviour
	{
		private Dictionary<MovableConditionType,MovableCondition> _map;
		private MovableCondition _curMovableCondition;
		public MovableCondition CurMovableCondition{get{return _curMovableCondition;}}
		private CharacterMotor _motor;
		private GameObjectController _gameObjectController;
		public GameObjectController gameObjectController{get{return _gameObjectController;}}
		private bool inited = false;
//		void Awake()
//		{
//			_motor = GetComponent<CharacterMotor>();
//			_gameObjectController = GetComponent<GameObjectController>();
//			_map = new Dictionary<MovableConditionType, MovableCondition>();
//			InitMovableCondition();
//		}

		public void InitControllerInfo()
		{
			_motor = GetComponent<CharacterMotor>();
			_gameObjectController = GetComponent<GameObjectController>();
			_map = new Dictionary<MovableConditionType, MovableCondition>();
			InitMovableCondition();
			inited = true;
		}

		public void RegisterMovableCondition(MovableCondition movableCondition)
		{
			if(!_map.ContainsKey(movableCondition.movableConditionType))
			{
				movableCondition.SetMovableController(this);
				_map.Add(movableCondition.movableConditionType,movableCondition);
			}
		}

		public void CanControl(bool canControl)
		{
			_motor.canControl = canControl;
		}

		public MovableCondition GetMovableCondition(MovableConditionType type)
		{
			MovableCondition condition;
			if(!_map.TryGetValue(type,out condition))throw new Exception("不存在类型为:"+type +"的movableCondition");
			return condition;
		}

		abstract protected void InitMovableCondition();

		public void ChangeMovableCondition(MovableConditionType type)
		{
			if(_curMovableCondition == null || _curMovableCondition.movableConditionType != type)
			{
				if(_curMovableCondition != null)
				{
					_curMovableCondition.Reset();
				}
				MovableCondition prevCondition = _curMovableCondition;
				_curMovableCondition = GetMovableCondition(type);
				_curMovableCondition.ChangeMovableConfig(_motor,prevCondition);
			}
		}

		public bool IsJumping()
		{
			return _motor.IsJumping();
		}

		public Vector3 GetVelocity()
		{
			return _motor.GetVelocity();
		}

		public void Move(Vector3 moveDirection)
		{
			_curMovableCondition.SetInputMoveDirection(moveDirection);
		}

		public void StopMove()
		{
			_curMovableCondition.SetInputMoveDirection(Vector3.zero);
		}

		public void Jump()
		{
			_curMovableCondition.SetInputJump(true);
		}

		public void StopJump()
		{
			_curMovableCondition.SetInputJump(false);
		}

		public void BeAttackMove(Vector3 direction)
		{
			_curMovableCondition.BeAttackMove(_motor,direction);
		}

		public bool IsGround()
		{
			return _motor.IsGrounded();
		}

		public void Update()
		{
			if(inited)
			{
				_curMovableCondition.OnUpdate(_motor);
			}
		}
	}
}

