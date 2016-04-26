using System;
using UnityEngine;
namespace MTB
{
	[System.Serializable]
	public class MovableWater : MovableCondition
	{
		#region implemented abstract members of MovableCondition

		public override MovableConditionType movableConditionType {
			get {
				return MovableConditionType.Water;
			}
		}

		#endregion
		[Range(0,1f)]
		public float waterReduceRate = 0.7f;
		public float riseGravity = -5f;
		public float GroundSwitchWaterSpeed = -2f;
		public MovableWater ()
		{
		}
		public override void SetInputMoveDirection (Vector3 inputMoveDirection)
		{
			_inputMoveDirection = inputMoveDirection * waterReduceRate;
		}

		public override void SetInputJump (bool inputJump)
		{
			_inputJump = inputJump;
		}

		public override void ChangeMovableConfig (CharacterMotor motor, MovableCondition prevCondition = null)
		{
			if(prevCondition != null && prevCondition.movableConditionType == MovableConditionType.Ground)
			{
				motor.movement.velocity.y = GroundSwitchWaterSpeed;
			}
			base.ChangeMovableConfig (motor, prevCondition);

		}

		public override void OnUpdate (CharacterMotor motor)
		{
			if(_inputJump || _inputMoveDirection != Vector3.zero)
			{
				motor.movement.gravity = riseGravity;
			}
			else
			{
				motor.movement.gravity = movement.gravity;
			}
			//强制将ground设为false
			motor.grounded = false;
			_inputJump = false;
			base.OnUpdate (motor);
		}

	}
}

