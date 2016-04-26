using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class BaseNPCAIState : INpcAIState
    {
        protected IAIComponent _npcAIComponent;
        protected IAIContext _aiContext;
        protected IAIStateManager _aiStateManager;
        protected GameObject _host;

        public BaseNPCAIState(IAIComponent npcAIComponent)
        {
            this._npcAIComponent = npcAIComponent;
            this._aiContext = npcAIComponent as IAIContext;
            this._aiStateManager = npcAIComponent as IAIStateManager;
            this._host = npcAIComponent.host();
            init();
        }

        protected virtual void init()
        {
            registerEvt();
        }

        public virtual AIStateType getType()
        {
            Debug.LogError("getType()：必须被重写");
            return AIStateType.IDEL;
        }

        public BaseNPCAIComponent getNPCAIComponent()
        {
            return _npcAIComponent as BaseNPCAIComponent;
        }

        protected virtual void registerEvt()
        {
        }

        protected virtual void unRegisterEvt()
        {
        }

        protected virtual bool isInBound(GameObject target, int minBound, int maxBound)
        {
            if (minBound < 0 && maxBound < 0)
                return true;
            float dis = Vector3.Distance(target.transform.position, _host.transform.position);
            if (dis < minBound || dis > maxBound)
                return false;

            return true;
        }

        public virtual void stateIn() { }

        public virtual void stateOut() { }

        public virtual AIStateType onThink()
        {
            return AIStateType.NONE;
        }

        protected bool isInWater()
        {
            if (getNPCAIComponent().hostController().gameObjectState.InBlock.BlockType == BlockType.StillWater || getNPCAIComponent().hostController().gameObjectState.InBlock.BlockType == BlockType.FlowingWater ||
                getNPCAIComponent().hostController().gameObjectState.StandBlock.BlockType == BlockType.StillWater || getNPCAIComponent().hostController().gameObjectState.StandBlock.BlockType == BlockType.FlowingWater
                )
            {
                return true;
            }
            return false;
        }

        public IAIComponent getAIComponent()
        {
            return this._npcAIComponent;
        }

        public IAIContext getAIContext()
        {
            return this._aiContext;
        }

        public virtual void dispose()
        {
            unRegisterEvt();
            this._npcAIComponent = null;
            this._aiContext = null;
            this._aiStateManager = null;
            this._host = null;
        }



    }
}
