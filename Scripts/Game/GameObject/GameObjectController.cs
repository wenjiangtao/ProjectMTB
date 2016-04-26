using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	[RequireComponent(typeof(MovableController))]
	[RequireComponent(typeof(BaseAttributes))]
	public class GameObjectController : MonoBehaviour
	{
		public delegate void BeAttackStartHandler();
		public delegate void BeAttackEndHandler();
		public delegate void ChangedChunkHandler(GameObjectController controller,Chunk oldChunk,Chunk newChunk);

		public event BeAttackStartHandler On_BeAttackStart;
		public event BeAttackEndHandler On_BeAttackEnd;
		public event ChangedChunkHandler On_ChangeChunk;


		protected GOActionController _goActionController;
		public GOActionController goActionController{get{return _goActionController;}}
		protected MovableController _movableController;
		public MovableController movableController{get{return _movableController;}}
		protected GameObjectState _gameObjectState;
		public GameObjectState gameObjectState{get{return _gameObjectState;}}
		protected GameObjectInputState _gameObjectInputState;
		public GameObjectInputState gameObjectInputState{get{return _gameObjectInputState;}}
		private BaseAttributes _baseAttribute;
		public BaseAttributes baseAttribute{get{return _baseAttribute;}}

		protected ObjectView _objectView;
		public ObjectView objectView{get{return _objectView;}}

		private BeAttackCheck _beAttackCheck;

		private bool inited = false;

//		protected virtual void Awake()
//		{
//			_movableController = GetComponent<MovableController>();
//			_baseAttribute = GetComponent<BaseAttributes>();
//			_world = World.world;
//			_objectView = transform.FindChild("ObjectView").GetComponent<ObjectView>();
//			_beAttackCheck = new BeAttackCheck(0.5f);
//			IsRead = false;
//		}

//		protected virtual void Start(){
//			InitState();
//			IsRead = true;
//		}

		public void InitControllerInfo()
		{

			_baseAttribute = GetComponent<BaseAttributes>();
			_objectView = transform.FindChild("ObjectView").GetComponent<ObjectView>();
			_beAttackCheck = new BeAttackCheck(0.5f);
			ResetPos();
			InitState();
			InitActionController();
			InitMovableController();
			InitOtherInfo();
			inited = true;
		}

		protected virtual void ResetPos(){}

		protected virtual void InitActionController(){}

		protected virtual void InitMovableController()
		{
			_movableController = GetComponent<MovableController>();
			_movableController.InitControllerInfo();
		}

		protected virtual void InitOtherInfo(){}

		public virtual void ChangeNetObj(bool netObj)
		{
			if(this.baseAttribute.isNetObj != netObj)
			{
				this.baseAttribute.isNetObj = netObj;
				_goActionController.ChangeNetObj(netObj);
			}
		}

		public virtual void ShowAvatar(bool enableAnimator = false)
		{
			if(enableAnimator)
			{
				_objectView.ShowAvatar();
			}
			else
			{
				_objectView.Show();
			}
		}

		public virtual void HideAvatar(bool enableAnimator = false)
		{
			if(enableAnimator)
			{
				_objectView.HideAvatar();
			}
			else
			{
				_objectView.Hide();
			}
		}

		public virtual void ChangeObjectView(ObjectView objectView)
		{
			HideAvatar();
			_objectView = objectView;
			ShowAvatar();
		}

		protected virtual void InitState()
		{
			_gameObjectState = new GameObjectState(this);
			_gameObjectInputState = new GameObjectInputState(this);
		}

		protected virtual void OnDestroy()
		{
			_gameObjectState.Dispose();
		}

		public void AcceptInput(bool canDoAction)
		{
			_movableController.CanControl(canDoAction);
		}

		public bool DoAction(int actionId,ActionParam param = null)
		{
			return _goActionController.DoAction(actionId,param);
		}

		public void Move(Vector3 direction)
		{
			_gameObjectInputState.moveDirection = direction;
			_movableController.Move(direction);
			_goActionController.ChangeActionSpeed(_movableController.GetVelocity().magnitude);
		}

		public void StopMove()
		{
			_gameObjectInputState.moveDirection = Vector3.zero;
			_movableController.StopMove();
			_goActionController.ChangeActionSpeed(0);
		}

		public void Jump()
		{
			_gameObjectInputState.jump = true;
			_movableController.Jump();
		}

		public void StopJump()
		{
			_gameObjectInputState.jump = false;
			_movableController.StopJump();
		}

		public virtual void BeAttack(BeAttackParam param)
		{
			if(_beAttackCheck.BeAttack())
			{
				movableController.BeAttackMove(param.attackDirection);
				if(On_BeAttackStart != null)On_BeAttackStart();
			}
		}

		public void ChangeChunkPos(Chunk oldChunk,Chunk newChunk)
		{
			if(On_ChangeChunk != null)
			{
				On_ChangeChunk(this,oldChunk,newChunk);
			}
		}

		public virtual void Update()
		{
			if(inited)
			{
				_gameObjectState.UpdateState();
				UpdateMovableCondition();
				if(_beAttackCheck.Update())
				{
					if(On_BeAttackEnd != null)On_BeAttackEnd();
				}
			}
		}

		protected virtual void UpdateMovableCondition()
		{
			if((_gameObjectState.InBlock.BlockType == BlockType.StillWater 
			    || _gameObjectState.InBlock.BlockType == BlockType.FlowingWater)
			   && _movableController.CurMovableCondition.movableConditionType != MovableConditionType.Water)
			{
				_movableController.ChangeMovableCondition(MovableConditionType.Water);
			}
			else if(!(_gameObjectState.InBlock.BlockType == BlockType.StillWater 
			          || _gameObjectState.InBlock.BlockType == BlockType.FlowingWater) && _movableController.CurMovableCondition.movableConditionType != MovableConditionType.Ground)
			{
				_movableController.ChangeMovableCondition(MovableConditionType.Ground);
			}
		}

		public class BeAttackCheck
		{
			private float _beAttackTime;
			private bool _beAttack;
			private float _curBeAttackTime;
			public BeAttackCheck(float beAttackTime)
			{
				_beAttackTime = beAttackTime;
				_curBeAttackTime = 0;
				_beAttack = false;
			}

			public bool BeAttack()
			{
				if(!_beAttack)
				{
					_beAttack = true;
					_curBeAttackTime = 0;
					return true;
				}
				return false;
			}

			public bool Update()
			{
				if(_beAttack && _curBeAttackTime < _beAttackTime)
				{
					_curBeAttackTime+=Time.deltaTime;
					return false;
				}
				else if(_beAttack)
				{
					_beAttack = false;
					return true;
				}
				return false;
			}
		}
	}

	public class GameObjectState{
		public bool IsGround{get{return _controller.movableController.IsGround();}}
		protected Block _standBlock = Block.NullBlock;
		public Block StandBlock{get{return _standBlock;}}
		protected Block _inBlock = Block.NullBlock;
		public Block InBlock{get{return _inBlock;}}
		protected int _sunLightLevel;
		public int SunLightLevel{get{return _sunLightLevel;}}
		protected int _blockLightLevel;
		public int BlockLightLevel{get{return _blockLightLevel;}}

		protected GameObjectController _controller;
		protected WorldPos _curPos = new WorldPos(0,0,0);
		public WorldPos CurPos{get{return _curPos;}}
		protected Chunk _attachChunk;
		public Chunk attachChunk{get{return _attachChunk;}}
		public GameObjectState(GameObjectController controller)
		{
			_controller = controller;
			_curPos = Terrain.GetWorldPos(_controller.transform.position);
			CheckAndResetPos();
			Chunk chunk = World.world.GetChunk(_curPos.x,0,_curPos.z);
			AttachChunk(chunk);
			WorldPos chunkPos = chunk.worldPos;
			_inBlock = _attachChunk.GetBlock(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
			_standBlock = _attachChunk.GetBlock(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y - 1,_curPos.z - chunkPos.z,true);
			_blockLightLevel = _attachChunk.GetBlockLight(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
			_sunLightLevel = _attachChunk.GetSunLight(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
			InitRenderLight(_blockLightLevel,_sunLightLevel);
		}

		public void UpdateState()
		{
			WorldPos pos = Terrain.GetWorldPos(_controller.transform.position);
			if(!_curPos.EqualOther(pos))
			{
				_curPos = pos;
				CheckAndResetPos();
				WorldPos chunkPos = Terrain.GetChunkPos(_curPos.x,0,_curPos.z);
				if(!chunkPos.EqualOther(_attachChunk.worldPos))
				{
					Chunk chunk = World.world.GetChunk(chunkPos.x,chunkPos.y,chunkPos.z);
					AttachChunk(chunk);
				}

				_inBlock = _attachChunk.GetBlock(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
				_standBlock = _attachChunk.GetBlock(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y - 1,_curPos.z - chunkPos.z,true);
				_blockLightLevel = _attachChunk.GetBlockLight(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
				_sunLightLevel = _attachChunk.GetSunLight(_curPos.x - chunkPos.x,_curPos.y - chunkPos.y,_curPos.z - chunkPos.z,true);
			}
			UpdateRenderLight(_sunLightLevel,_blockLightLevel);
		}

		protected virtual void CheckAndResetPos()
		{
			if(_curPos.y <= 0 || _curPos.y > 256)
			{
				WorldPos pos = Terrain.GetWorldPos(_controller.transform.position);
				int height = WorldConfig.Instance.heightCap;
				for (int y = height - 1; y >= 0; y--)
				{
					if (World.world.GetBlock(pos.x, y + World.MinHeight, pos.z).BlockType != BlockType.Air)
					{
						height = y + World.MinHeight;
						break;
					}
				}
				_controller.transform.position = new Vector3(_controller.transform.position.x, (float)height + 1, _controller.transform.position.z);
				_curPos = Terrain.GetWorldPos(_controller.transform.position);
			}
		}

		protected virtual void UpdateRenderLight(int sunLightLevel,int blockLightLevel)
		{
			_controller.objectView.UpdateLight(sunLightLevel,blockLightLevel);
		}

		protected virtual void InitRenderLight(int sunLightLevel,int blockLightLevel)
		{
			_controller.objectView.InitLight(sunLightLevel,blockLightLevel);
		}

		protected virtual void AttachChunk(Chunk chunk)
		{
			Chunk oldChunk = _attachChunk;
			_attachChunk = chunk;
			if(oldChunk != null)
				_controller.ChangeChunkPos(oldChunk,_attachChunk);
		}

		public virtual void Dispose()
		{
			_attachChunk = null;
		}
	}

	public class GameObjectInputState{

		private Vector3 _moveDirection;
		public Vector3 moveDirection{
			get{return _moveDirection;}
			set{
				if((value != Vector3.zero && _moveDirection == Vector3.zero))
				{
					_moveDirection = value;
					UpdateAction(InputType.Joystick,(int)InputJoystickType.Zero,(int)InputJoystickType.NoZero);
				}
				else if(value == Vector3.zero && _moveDirection != Vector3.zero)
				{
					_moveDirection = value;
					UpdateAction(InputType.Joystick,(int)InputJoystickType.NoZero,(int)InputJoystickType.Zero);
				}
				else
				{
					_moveDirection = value;
				}
			}
		}
		private bool _jump;
		public bool jump{
			get{return _jump;}
			set{
				if(!_jump&& value)
				{
					_jump = value;
					UpdateAction(InputType.Button,(int)InputButtonType.JumpDwon,(int)InputButtonType.JumpUp);
				}
				else if(_jump && !value)
				{
					_jump = value;
					UpdateAction(InputType.Button,(int)InputButtonType.JumpUp,(int)InputButtonType.JumpDwon);
				}
				else{
					_jump = value;
				}
			}
		}

		private GameObjectController _controller;
		public GameObjectInputState(GameObjectController controller)
		{
			_moveDirection = Vector2.zero;
			_jump = false;
			_controller = controller;
		}

		public virtual void UpdateAction(InputType inputType,int inputValue,int oldValue)
		{
		}
	}
}

