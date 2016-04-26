using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class BaseNPCAIComponent : BaseAIComponent
    {
        protected bool _isInited;
        protected int _npcId;
        protected NPCAttributes _attributes;

        public BaseNPCAIComponent()
            : base()
        {
        }

        public override void attach(GameObject haveActionObj)
        {
            base.attach(haveActionObj);
            _attributes = haveActionObj.GetComponent<NPCAttributes>();
            _npcId = _attributes.NPCId;
            System.Random random = new System.Random();
            float startfaceDirx = random.Next(2) - 1;
            float startfaceDirz = random.Next(2) - 1;
            _host.GetComponent<AutoMoveController>().faceDir(new Vector2(startfaceDirx, startfaceDirz));
        }

        public override GameObject seachTarget(bool checkHeight = true)
        {
            return null;
        }

        public override void run()
        {
            initEvent();
            if (!this._isInited)
            {
                initNPCAIState();
                this._isInited = true;
            }
            base.run();
        }

        public override void pause()
        {
            removeEvent();
            if (isRunning())
            {
                base.pause();
                runAIState(AIStateType.FREE);
            }
        }

        public override void runAIState(AIStateType key)
        {
            if (canRunAIState(key))
            {
                base.runAIState(key);
            }
            else
            {
                base.runAIState(AIStateType.FREE);
            }
        }

        public override void onTick()
        {
            base.onTick();
            if (_hostController.gameObjectState.InBlock.BlockType == BlockType.StillWater || _hostController.gameObjectState.InBlock.BlockType == BlockType.FlowingWater)
            {
                _hostController.Jump();
            }
        }

        protected virtual void initNPCAIState()
        {
            Debug.LogError("子类必须重写initNPCAIState方法！");
        }

        protected virtual void registerNPCAIState(INpcAIState npcAIState)
        {
            registerAIState(npcAIState.getType(), npcAIState);
        }

        protected virtual bool canRunAIState(AIStateType key)
        {
            return true;
        }

        protected override void initAIData()
        { }

        private void initEvent()
        {
            EventManager.RegisterEvent(EventMacro.TASK_FINISH_STEP, onTaskStepFinish);
        }

        private void removeEvent()
        {
            EventManager.UnRegisterEvent(EventMacro.TASK_FINISH_STEP, onTaskStepFinish);
        }

        private void onTaskStepFinish(params object[] paras)
        {
            int taskId = Convert.ToInt32(paras[0]);
            int stepId = Convert.ToInt32(paras[1]);
            if (taskId == _attributes.taskId && stepId == _attributes.stepId)
            {
                if (_attributes.NPCId == 3)
                {
                    HasActionObjectManager.Instance.npcManager.removeObj(_attributes.aoId);
                    return;
                }
                _host.GetComponent<NPCAttributes>().taskId = 0;
                _host.GetComponent<NPCAttributes>().stepId = 0;
            }

        }

        public bool hasState(AIStateType type)
        {
            return _stateMap.ContainsKey(type);
        }

        public GameObjectController hostController()
        {
            return _hostController;
        }
    }
}
