using System;
using UnityEngine;
namespace MTB
{
	[System.Serializable]
	abstract public class MovableCondition
	{
		abstract public MovableConditionType movableConditionType{get;}
		public bool canControl = true;
		public bool useFixedUpdate = true;
		public CharacterMotorMovement movement = new CharacterMotorMovement();
		public CharacterMotorJumping jumping = new CharacterMotorJumping();
		public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
		public CharacterMotorSliding sliding = new CharacterMotorSliding();


		public float maxBeAttackMoveSpeed = 5f;
		public bool needJump = true;

		protected Vector3 _inputMoveDirection;
		protected bool _inputJump;
		protected MovableController _movableController;
		public MovableCondition ()
		{
			Reset();
		}

		public void SetMovableController(MovableController controller)
		{
			_movableController = controller;
		}

		public virtual void SetInputMoveDirection(Vector3 inputMoveDirection)
		{
			this._inputMoveDirection = inputMoveDirection;
		}

		public virtual void SetInputJump(bool inputJump)
		{
			this._inputJump = inputJump;
		}

		public virtual void BeAttackMove(CharacterMotor motor,Vector3 direction)
		{
			Vector3 speed = direction * maxBeAttackMoveSpeed;
			if(needJump && motor.IsGrounded())
			{
				speed += motor.jumping.jumpDir * motor.CalculateJumpVerticalSpeed(motor.jumping.baseHeight);
			}
			motor.SetVelocity(speed);
		}

		public virtual void ChangeMovableConfig(CharacterMotor motor,MovableCondition prevCondition = null)
		{

			motor.canControl = canControl;
			motor.useFixedUpdate = useFixedUpdate;

			motor.movement.maxForwardSpeed = movement.maxForwardSpeed;
			motor.movement.maxSidewaysSpeed = movement.maxSidewaysSpeed;
			motor.movement.maxBackwardsSpeed = movement.maxBackwardsSpeed;

			motor.movement.slopeSpeedMultiplier = movement.slopeSpeedMultiplier;

			motor.movement.maxGroundAcceleration = movement.maxGroundAcceleration;
			motor.movement.maxAirAcceleration = movement.maxAirAcceleration;

			motor.movement.gravity = movement.gravity;
			motor.movement.maxFallSpeed = movement.maxFallSpeed;
			motor.movement.maxRiseSpeed = movement.maxRiseSpeed;

			motor.jumping.enabled = jumping.enabled;
			motor.jumping.baseHeight = jumping.baseHeight;
			motor.jumping.extraHeight = jumping.extraHeight;
			motor.jumping.perpAmount = jumping.perpAmount;
			motor.jumping.steepPerpAmount = jumping.steepPerpAmount;

			motor.movingPlatform.enabled = movingPlatform.enabled;
			motor.movingPlatform.movementTransfer = movingPlatform.movementTransfer;

			motor.sliding.enabled = sliding.enabled;
			motor.sliding.slidingSpeed = sliding.slidingSpeed;
			motor.sliding.sidewaysControl = sliding.sidewaysControl;
			motor.sliding.speedControl = sliding.speedControl;

		}

		public virtual void OnUpdate(CharacterMotor motor)
		{
			motor.inputMoveDirection = _movableController.transform.rotation * _inputMoveDirection;
			motor.inputJump = _inputJump;
		}

		public void Reset()
		{
			_inputMoveDirection = Vector3.zero;
			_inputJump = false;
		}
	}
}

