using System;
using UnityEngine;
namespace MTB
{
	[System.Serializable]
	public class MovableFree : MovableCondition
	{
		#region implemented abstract members of MovableCondition

		public override MovableConditionType movableConditionType {
			get {
				return MovableConditionType.Free;
			}
		}

		#endregion
		[Range(0,20)]
		public float speed = 10;
		private float raiseGravity = 0;

		public MovableFree ()
		{
		}

		public override void SetInputMoveDirection (UnityEngine.Vector3 inputMoveDirection)
		{
			if(_inputJump)
			{
				raiseGravity = -inputMoveDirection.z * speed;
				this._inputMoveDirection = Vector3.zero;
			}
			else
			{
				this._inputMoveDirection = inputMoveDirection;
				raiseGravity = 0;
			}
		}

		public override void SetInputJump (bool inputJump)
		{
			base.SetInputJump (inputJump);
		}

		public override void OnUpdate (CharacterMotor motor)
		{
			motor.inputMoveDirection = _movableController.transform.rotation * _inputMoveDirection;
			motor.inputJump = false;
			//强制将ground设为false
			motor.grounded = false;
			motor.movement.gravity = raiseGravity;
			if(Mathf.Approximately(0,raiseGravity))
			{
				Vector3 v = motor.GetVelocity();
				if(!Mathf.Approximately(0,v.y))
				{
					motor.SetVelocity(new Vector3(v.x,0,v.z));
				}
					
			}
		}
	}
}

