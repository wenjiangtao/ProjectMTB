using System;
using UnityEngine;
namespace MTB
{
	[RequireComponent(typeof(MonsterMovableController))]
	[RequireComponent(typeof(MonsterAttributes))]
	public class GOMonsterController : GameObjectController
	{
		public delegate void BeFeedHandler();
		public event BeFeedHandler On_BeFeed;
		public MonsterAttributes monsterAttribute{get{return baseAttribute as MonsterAttributes;}}

		private bool canBeAttack = true;

//		protected override void Start ()
//		{
//			InitState();
//			_goActionController = new GOMonsterActionController(this);
//			_goActionController.BindAnimatorController(objectView.animatorController);
//		}

		protected override void InitState ()
		{
			_gameObjectState = new MonsterState(this);
			_gameObjectInputState = new GameObjectInputState(this);
		}

		protected override void InitActionController ()
		{
			_goActionController = new GOMonsterActionController(this);
			_goActionController.BindAnimatorController(objectView.animatorController);
		}

		protected override void InitOtherInfo ()
		{
			MonsterBasicData monsterdata = DataManagerM.Instance.getManager(GameObjectTypes.MONSTER).getData(monsterAttribute.monsterId, DataTypes.Basic) as MonsterBasicData;
			gameObject.GetComponent<AIContainer>().AIComponent = MonsterAIFactory.getAIComponent(monsterdata.aiComponent);
			//如果不是网络对象，开启ai
			if(!baseAttribute.isNetObj)
			{
				gameObject.GetComponent<AIContainer>().AIComponent.run();
			}
		}

		protected override void OnDestroy ()
		{
			gameObject.GetComponent<AIContainer>().AIComponent.pause();
			base.OnDestroy ();
		}

		public override void ChangeNetObj (bool netObj)
		{
			base.ChangeNetObj (netObj);
			//如果改变成本地对象，那么将ai开启，否则关闭（网络对象的控制是通过同步来实现的）
			if(!netObj)
			{
				gameObject.GetComponent<AIContainer>().AIComponent.run();
			}else
			{
				gameObject.GetComponent<AIContainer>().AIComponent.pause();
			}
		}

		public virtual void BeFeed()
		{
			if(On_BeFeed != null)On_BeFeed();
		}

		public override void ShowAvatar (bool enableAnimator = false)
		{
			canBeAttack = true;
			base.ShowAvatar (enableAnimator);
		}

		public override void HideAvatar (bool enableAnimator = false)
		{
			canBeAttack = false;
			base.HideAvatar (enableAnimator);
		}

		public override void BeAttack (BeAttackParam param)
		{
			if(!canBeAttack)return;
			base.BeAttack (param);
		}

		public EntityData GetEntityData()
		{
			EntityData entityData = new EntityData();
			entityData.type = EntityType.MONSTER;
			entityData.id = monsterAttribute.monsterId;
			entityData.pos = this.transform.position;
			return entityData;
		}
	}

	public class MonsterState : GameObjectState
	{
		public MonsterState(GOMonsterController controller)
			:base(controller)
		{
		}
		public GOMonsterController controller{get{return _controller as GOMonsterController;}}

		protected override void AttachChunk(Chunk chunk)
		{
			if(chunk == null || !chunk.isGenerated)
			{
				HasActionObjectManager.Instance.monsterManager.removeObj(controller.gameObject);
				return;
			}
			base.AttachChunk(chunk);
		}

		public override void Dispose ()
		{
			_attachChunk = null;
		}
		
		private EntityData GetEntityData()
		{
			EntityData data = new EntityData();
			data.id = controller.monsterAttribute.monsterId;
			data.pos = controller.transform.position;
            data.type = EntityType.MONSTER;
			return data;
		}
	}
}

