using UnityEngine;
using System.Collections;
namespace MTB
{
    public class BaseChaseComponent : IChaseComponent
    {
        protected GameObject _host;
        protected GameObject _target;
        protected MoveType _moveType = MoveType.WALK;

        protected int _minChase;
        protected int _maxChase;
        protected int _maxDistance;

        protected Vector3 _desPoint = new Vector3(0, 0, 0);
        protected bool _isMoveEnd;

        public BaseChaseComponent(GameObject host)
        {
            this._host = host;
        }

        public void updateTarget(GameObject target)
        {
            this._target = target;
            startMove();
        }

        public void setMoveType(MoveType type)
        {
            this._moveType = type;
        }

        public void setChaseParams(int minChase, int maxChase, int maxDistance)
        {
            this._minChase = minChase;
            this._maxChase = maxChase;
            this._maxDistance = maxDistance;
            if (this._minChase > this._maxChase)
            {
                Debug.LogError("参数设置错误");
            }
        }

        public virtual bool onChasing()
        {
            Debug.LogError("BaseChaseTarget:onChasing() 必须被重写");
            return false;
        }

        public virtual bool onLose()
        {
            return Vector3.Distance(_host.transform.position, _target.transform.position) >= _maxDistance;
        }

        protected virtual void startMove()
        {
            if (_desPoint == null)
                return;
            EventManager.RegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
            _host.GetComponent<AutoMoveController>().startChaseObject(_target.transform);
            _isMoveEnd = false;
        }

        protected virtual void stopMove()
        {
            _host.GetComponent<AutoMoveController>().stopChase();
            EventManager.UnRegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
            _isMoveEnd = true;
        }

        protected virtual void onMoveToTarget(params object[] paras)
        {
            if ((paras[0] as GameObject).GetInstanceID() == _host.GetInstanceID())
            {
                EventManager.UnRegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
                _isMoveEnd = true;
                onMoveEnd();
            }
        }

        protected virtual void onMoveEnd()
        { }

        public virtual bool isChaseTarget()
        {
            return (Vector3.Distance(_host.transform.position, _target.transform.position) < this._maxChase) &&
                 (Vector3.Distance(_host.transform.position, _target.transform.position) > this._minChase);
        }

        public virtual void reset()
        {
            stopMove();
            this._target = null;
            this._moveType = MoveType.WALK;
            this._minChase = 0;
            this._maxChase = 0;
        }

        public virtual void dispose()
        {
            reset();
            this._host = null;
        }
    }
}
