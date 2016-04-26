using UnityEngine;
using System.Collections;
namespace MTB
{

    public class LodgedAfterRoamState : BaseMonsterAIState
    {
        private int _stateTime;
        private int _showTime;
        private int _state;

        public LodgedAfterRoamState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            _state = 1;
            _showTime = 100;
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
            if (_state == 1 && _showTime <= 0)
            {
                _state = 2;
            }
            if (_state == 2)
            {
                _state = 3;
                getMonsterAIComponent().hostController().ShowAvatar(true);
            }
            if (_stateTime <= 0)
            {
                return AIStateType.IDEL;
            }
            //todo出现动作
            _stateTime--;
            _showTime--;
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.AFTERROAM;
        }
    }
}
