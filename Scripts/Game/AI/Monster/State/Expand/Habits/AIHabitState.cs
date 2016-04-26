using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIHabitState : BaseMonsterAIState
    {
        //动作没有结束事件之前用时间代替
        private int _stateTime;

        public AIHabitState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            _stateTime = 400;
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
            if (_stateTime <= 0)
            {
                return AIStateType.FREE;
            }
            _stateTime--;
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.HABIT;
        }
    }
}
