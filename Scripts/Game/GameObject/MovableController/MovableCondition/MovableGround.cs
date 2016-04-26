using System;
using UnityEngine;
namespace MTB
{
	[System.Serializable]
	public class MovableGround : MovableCondition
	{
		#region implemented abstract members of MovableCondition

		public override MovableConditionType movableConditionType {
			get {
				return MovableConditionType.Ground;
			}
		}

		#endregion
		public float WaterSwitchGroundSpeed = 2f;

		public bool enableAutoJump = false;
		public ObstacleDetector lowDetector;
		public ObstacleDetector highDetector;
		public MovableGround ()
		{
		}

		public override void ChangeMovableConfig (CharacterMotor motor, MovableCondition prevCondition = null)
		{
			if(prevCondition != null && prevCondition.movableConditionType == MovableConditionType.Water)
			{
				motor.movement.velocity.y = WaterSwitchGroundSpeed;
			}
			base.ChangeMovableConfig (motor, prevCondition);
		}

		public override void SetInputMoveDirection (UnityEngine.Vector3 inputMoveDirection)
		{
			if(enableAutoJump && lowDetector != null && highDetector!= null)
			{
				if(lowDetector.inCollision && !highDetector.inCollision && !_movableController.IsJumping() && inputMoveDirection.z > 0)
				{
					_movableController.gameObjectController.Jump();
				}
//				else if(_movableController.IsJumping())
//				{
//					_movableController.gameObjectController.StopJump();
//				}
			}
			_inputMoveDirection = inputMoveDirection;
			if (_inputMoveDirection != Vector3.zero) {
				float directionLength = _inputMoveDirection.magnitude;
				_inputMoveDirection = _inputMoveDirection / directionLength;
				
				directionLength = Mathf.Min(1, directionLength);
				
				directionLength = directionLength * directionLength;

				_inputMoveDirection = _inputMoveDirection * directionLength;
			}
		}

		public override void OnUpdate (CharacterMotor motor)
		{
			if(!_movableController.IsGround())
			{
				_movableController.gameObjectController.StopJump();
			}
			base.OnUpdate (motor);
		}

	}
}

