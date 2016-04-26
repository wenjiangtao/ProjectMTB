using UnityEngine;
using System.Collections;
namespace MTB
{

    public class LodgedPreRoamState : BaseMonsterAIState
    {
        private int _beforeHideTime;
        private int _stateTime;
        private int _state;
        public LodgedPreRoamState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            _state = 1;
            _beforeHideTime = 100;
            _stateTime = 200;
            base.stateIn();
            this._monsterAIComponent.setTarget(null);
            this._host.GetComponent<AutoMoveController>().cancelAutoMove();
            getMonsterAIComponent().hostController().DoAction(DataManagerM.Instance.getMonsterDataManager().getActionData(_host).specialAction[0]);
        }

        public override void stateOut()
        {
            getMonsterAIComponent().hostController().DoAction(DataManagerM.Instance.getMonsterDataManager().getActionData(_host).defaultAction);
            base.stateOut();
        }

        public override AIStateType onThink()
        {
            if (_state == 1 && _beforeHideTime <= 0)
            {
                _state = 2;
            }
            if (_state == 2)
            {
                _state = 3;
                getMonsterAIComponent().hostController().HideAvatar(true);
            }
            if (_stateTime <= 0)
            {
                return AIStateType.ROAM;
            }
            //todo下降动作
            _stateTime--;
            _beforeHideTime--;
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.PREROAM;
        }
    }
}
