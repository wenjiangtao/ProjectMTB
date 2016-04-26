using System;
using UnityEngine;
namespace MTB
{
    [RequireComponent(typeof(NPCMovableController))]
    [RequireComponent(typeof(NPCAttributes))]
    public class GONPCController : GameObjectController
    {
        public delegate void BeTouch();
        public NPCAttributes npcAttribute { get { return baseAttribute as NPCAttributes; } }

        protected override void InitState()
        {
            _gameObjectState = new NPCState(this);
            _gameObjectInputState = new GameObjectInputState(this);
        }

        protected override void InitActionController()
        {
            _goActionController = new GONpcActionController(this);
            _goActionController.BindAnimatorController(objectView.animatorController);
        }

        protected override void InitOtherInfo()
        {
            if (gameObject.GetComponent<AIContainer>() == null)
                gameObject.AddComponent<AIContainer>();
            gameObject.GetComponent<AIContainer>().AIComponent = new DefaultNpcAIComponent();
            gameObject.GetComponent<AIContainer>().AIComponent.run();
        }

        protected override void OnDestroy()
        {
            gameObject.GetComponent<AIContainer>().AIComponent.pause();
            base.OnDestroy();
        }


        public override void ShowAvatar(bool enableAnimator = false)
        {
            base.ShowAvatar(enableAnimator);
        }

        public override void HideAvatar(bool enableAnimator = false)
        {
            base.HideAvatar(enableAnimator);
        }

        public EntityData GetEntityData()
        {
            EntityData entityData = new EntityData();
            entityData.type = EntityType.NPC;
            entityData.id = npcAttribute.NPCId;
            entityData.pos = this.transform.position;
            entityData.exData.Clear();
            entityData.exData.Add(npcAttribute.taskId);
            entityData.exData.Add(npcAttribute.stepId);
            return entityData;
        }
    }

    public class NPCState : GameObjectState
    {
        public NPCState(GONPCController controller)
            : base(controller)
        {
        }

        public GONPCController controller { get { return _controller as GONPCController; } }

        protected override void AttachChunk(Chunk chunk)
        {
            if (chunk == null || !chunk.isGenerated)
            {
                HasActionObjectManager.Instance.npcManager.removeObj(controller.gameObject);
                return;
            }
            base.AttachChunk(chunk);
        }

        private EntityData GetEntityData()
        {
            EntityData data = new EntityData();
            data.id = controller.npcAttribute.NPCId;
            data.pos = controller.transform.position;
            data.exData.Clear();
            data.exData.Add(controller.npcAttribute.taskId);
            data.exData.Add(controller.npcAttribute.stepId);
            data.type = EntityType.NPC;
            return data;
        }
    }
}
