/*****************
 * 繁殖状态
 * ***************/
using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class AIBreedState : BaseMonsterAIState
    {
        private int WAITBREADTIME = 5;
        private float _breedTime;
        private int _breedDis;
        private int _cubId;
        private float _breedRate;
        private System.Random _random;

        public AIBreedState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
            _random = new System.Random();
            _breedRate = DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).breedRate;
        }

        public override void stateIn()
        {
            base.stateIn();
            _breedTime = 0;
            _cubId = DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).cubID;
            _breedDis = DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).breedDis;
            getMonsterAIComponent().hostController().DoAction(DataManagerM.Instance.getMonsterDataManager().getActionData(_host).defaultAction);
            if (DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).breedType == 1)
            {
                breed();
                return;
            }
            EventManager.RegisterEvent(EventMacro.ON_BREED, onBreed);
            EventManager.RegisterEvent(EventMacro.BREED_FINISH, onBreedFinish);
            EventManager.SendEvent(EventMacro.ON_BREED, _host);
        }

        public override void stateOut()
        {
            base.stateOut();
            EventManager.UnRegisterEvent(EventMacro.ON_BREED, onBreed);
            EventManager.UnRegisterEvent(EventMacro.BREED_FINISH, onBreedFinish);
        }

        public override AIStateType onThink()
        {
            _breedTime += Time.deltaTime;
            if (_breedTime >= WAITBREADTIME)
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.BREED;
        }

        private void onBreed(params object[] paras)
        {
            if (checkCanBreed(paras[0] as GameObject))
            {
                EventManager.UnRegisterEvent(EventMacro.ON_BREED, onBreed);
                EventManager.SendEvent(EventMacro.BREED_FINISH, _host, (paras[0] as GameObject).GetComponent<BaseAttributes>().aoId);
                this._aiStateManager.runAIState(AIStateType.FREE);
            }
        }

        private void onBreedFinish(params object[] paras)
        {
            if (checkCanBreed(paras[0] as GameObject) && Convert.ToInt32(paras[1]) == _host.GetComponent<BaseAttributes>().aoId)
            {
                EventManager.UnRegisterEvent(EventMacro.BREED_FINISH, onBreedFinish);
                breed();
            }
        }

        private void breed()
        {
            if (_random.Next(100) <= _breedRate * 100)
            {
                Vector3 birthPos = new Vector3(_host.transform.position.x, _host.transform.position.y, _host.transform.position.z);
				MonsterInfo info = new MonsterInfo();
				info.position = birthPos;
				info.aoId = AoIdManager.instance.getAoId();
				info.monsterId = _cubId;
				HasActionObjectManager.Instance.monsterManager.InitMonster(info);
            }
            this._aiStateManager.runAIState(AIStateType.FREE);
        }

        private bool checkCanBreed(GameObject target)
        {
            if (_host.GetComponent<BaseAttributes>().aoId == target.GetComponent<BaseAttributes>().aoId
            || _host.GetComponent<MonsterAttributes>().monsterId != target.GetComponent<MonsterAttributes>().monsterId)
                return false;
            if (Vector3.Distance(_host.transform.position, target.transform.position) >= _breedDis)
                return false;
            return true;
        }
    }
}
