using UnityEngine;
using System.Collections;
namespace MTB
{
    public class BaseRoamComponent : IRoamComponent
    {
        protected GameObject _host;

        protected Vector3 _centerPoint;

        protected MoveType _moveType;

        protected int _roamRadius;

        protected int _minRoamDis;

        protected int _idelTime;

        protected float _faceDirection;

        protected RoamType _roamType = RoamType.IDLE;

        protected int _currentTime;

        protected float _moveSpeed;

        public BaseRoamComponent(GameObject host)
        {
            this._host = host;
            _moveSpeed = DataManagerM.Instance.getMonsterDataManager().getAIData(_host).moveSpeed;
            setRoamCenterPoint(host.transform.position);
        }

        public void setMoveType(MoveType type)
        {
            this._moveType = type;
        }

        public void setRoamCenterPoint(Vector3 point)
        {
            this._centerPoint = point;
        }

        public void setRoamParams(int radius, int idleTime, int minRoamDis)
        {
            this._roamRadius = radius;
            this._idelTime = idleTime;
            this._minRoamDis = minRoamDis;
        }

        public virtual void setRoamType(RoamType type)
        {
            this._roamType = type;
            if (this._roamType == RoamType.WALK)
            {
                onStartWalk();
            }
            else if (this._roamType == RoamType.IDLE)
            {
                this._currentTime = 0;
                onStartIdle();
            }
            else if (this._roamType == RoamType.LODGED)
            {
                onSpecialRoam();
            }
        }

        public RoamType getRoamType()
        {
            return this._roamType;
        }

        public void onRoam()
        {
            this._faceDirection = this._host.transform.eulerAngles.y;

            if (this._roamType == RoamType.IDLE)
            {
                this._currentTime++;
                if (this._currentTime > this._idelTime)
                {
                    setRoamType(RoamType.WALK);
                }
            }

            if (this._roamType == RoamType.WALK)
            {
                onWalk();
            }
            else
            {
                onIdle();
            }
        }

        protected virtual void onStartIdle() { }

        protected virtual void onIdle() { }

        protected virtual void onStartWalk() { }

        protected virtual void onSpecialRoam() { }

        protected virtual void startWalk(Vector3 desPoint)
        {
            if (desPoint != null && _moveType == MoveType.WALK)
            {
                EventManager.RegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
                _host.GetComponent<AutoMoveController>().startAutoMove(desPoint, _moveSpeed);
            }
            else
            {
                onWalkEnd();
            }
        }

        protected virtual void onMoveToTarget(params object[] paras)
        {
            if ((paras[0] as GameObject).GetInstanceID() == _host.GetInstanceID())
            {
                EventManager.UnRegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
                onWalkEnd();
            }
        }

        protected virtual void onWalkEnd()
        {
            setRoamType(RoamType.IDLE);
        }

        protected virtual void stopMove()
        {
            _host.GetComponent<AutoMoveController>().cancelAutoMove();
            EventManager.UnRegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
        }

        protected virtual void onWalk() { }

        public float getFaceDirection()
        {
            return this._faceDirection;
        }

        public void reset()
        {
            stopMove();
            this._centerPoint.Set(0, 0, 0);
            this._moveType = MoveType.WALK;
            this._roamRadius = 0;
            this._idelTime = 0;
            this._roamType = RoamType.IDLE;
            this._currentTime = 0;
        }

        public void dispose()
        {
            if (_roamType == RoamType.WALK)
            {
                stopMove();
            }
            this._host = null;
        }
    }
}
