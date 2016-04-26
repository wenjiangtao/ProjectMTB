using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AutoMoveController : MonoBehaviour
    {

        private MTBPath _movePath;
        private int _moveStep = 0;
        private Vector3 _nextPoint;
        private GameObject _lookObj;
        private GameObjectController _controller;
        private Transform _chaseObject;
        private Transform _runAwayObject;
        private float _movespeed;
        private float _chaseMinDis;
        private float _runawayDis;

        protected bool isAutoMove = false;
        // Use this for initialization
        void Start()
        {
            _controller = gameObject.GetComponent<GameObjectController>();
            AddEventListener();
            if (_lookObj == null)
            {
                _lookObj = new GameObject();
                _lookObj.name = "lookObj";
                _lookObj.transform.parent = gameObject.transform;
            }
            _movespeed = DataManagerM.Instance.getMonsterDataManager().getAIData(gameObject).moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (isAutoMove)
            {
                if (_movePath != null && _movePath.pathData != null)
                {
                    moveToNextblock();
                }
            }
            else if (_chaseObject != null)
            {
                chasing();
            }
            else if (_runAwayObject != null)
            {
                runAway();
            }
        }

        void OnDestroy()
        {
            RemoveEventListener();
        }

        private void AddEventListener()
        {
            EventManager.RegisterEvent(EventMacro.AUTO_MOVE, OnAutoMove);
        }

        private void RemoveEventListener()
        {
            EventManager.UnRegisterEvent(EventMacro.AUTO_MOVE, OnAutoMove);
        }

        private void OnAutoMove(params object[] paras)
        {
            if (paras[0].ToString() == this.gameObject.GetComponent<BaseAttributes>().aoId.ToString())
            {
                startAutoMove((Vector3)paras[1]);
            }
        }

        public bool startAutoMove(Vector3 targetPosition, float speed = 1F)
        {
            _movespeed = speed;
            WorldPos startWorldPos = Terrain.GetWorldPos(gameObject.transform.position);
            Vector3 startPos = new Vector3(startWorldPos.x, startWorldPos.y, startWorldPos.z);
            _movePath = MTBPathFinder.Instance.GetPath(startPos, targetPosition, true);
            isAutoMove = true;
            _moveStep = 0;
            return _movePath.pathData != null;
        }

        public void cancelAutoMove()
        {
            endMove();
        }

        public void LodgedMove(Vector3 targetPosition)
        {
            _lookObj.transform.position = new Vector3(targetPosition.x, gameObject.transform.position.y, targetPosition.z);
            faceToNextBlock(_lookObj.transform);
            gameObject.transform.position = targetPosition;
        }

        private bool isMoveToBlock(Vector3 pos, float distance = 1F)
        {
            if (Mathf.Pow(gameObject.transform.position.x - pos.x, 2) + Mathf.Pow(gameObject.transform.position.z - pos.z, 2) <= Mathf.Pow(distance, 2))
            {
                return true;
            }
            return false;
        }

        private bool isMoveOutBlock(Vector3 pos, float distance = 1F)
        {
            if (Mathf.Pow(gameObject.transform.position.x - pos.x, 2) + Mathf.Pow(gameObject.transform.position.z - pos.z, 2) >= Mathf.Pow(distance, 2))
            {
                return true;
            }
            return false;
        }

        private void faceToNextBlock(Transform trans)
        {
            this.gameObject.transform.LookAt(trans);
        }

        private void moveToNextblock()
        {
            if (_movePath == null || _movePath.pathData == null)
            {
                _controller.StopMove();
                return;
            }
            _nextPoint = _movePath.pathData[_moveStep];
            _lookObj.transform.position = new Vector3(_nextPoint.x, gameObject.transform.position.y, _nextPoint.z);
            faceToNextBlock(_lookObj.transform);
            if (isMoveToBlock(_nextPoint))
            {
                _moveStep++;
                if (_moveStep >= _movePath.pathData.Length - 1)
                {
                    isAutoMove = false;
                    _controller.StopMove();
                    EventManager.SendEvent(EventMacro.MOVE_TO_DESPOINT, gameObject);
                    return;
                }
            }
            _controller.Move(new Vector3(0, 0, _movespeed));
        }

        public void endMove()
        {
            if (_controller != null)
            {
                _controller.StopMove();
            }
            isAutoMove = false;
            _movePath = null;
        }

        public void startChaseObject(Transform transform, float chasemindis = 2F)
        {
            _chaseMinDis = chasemindis;
            _chaseObject = transform;
        }

        public void startRunAway(Transform transform, float runawaydis = 20F)
        {
            _runawayDis = runawaydis;
            _runAwayObject = transform;
        }

        private void chasing()
        {
            _lookObj.transform.position = new Vector3(_chaseObject.position.x, gameObject.transform.position.y, _chaseObject.position.z);
            faceToNextBlock(_lookObj.transform);
            if (isMoveToBlock(_chaseObject.position, _chaseMinDis))
            {
                _controller.StopMove();
                EventManager.SendEvent(EventMacro.MOVE_TO_DESPOINT, gameObject);
                _chaseObject = null;
            }
            _controller.Move(new Vector3(0, 0, _movespeed));
        }

        private void runAway()
        {
            _lookObj.transform.position = new Vector3(
                gameObject.transform.position.x * 2 - _runAwayObject.transform.position.x,
                gameObject.transform.position.y,
                gameObject.transform.position.z * 2 - _runAwayObject.transform.position.z);
            faceToNextBlock(_lookObj.transform);
            if (isMoveOutBlock(_runAwayObject.position, _runawayDis))
            {
                _controller.StopMove();
                EventManager.SendEvent(EventMacro.MOVE_TO_DESPOINT, gameObject);
                _runAwayObject = null;
            }
            _controller.Move(new Vector3(0, 0, _movespeed * 3));
        }

        public void stopChase()
        {
            _controller.StopMove();
            _chaseObject = null;
        }

        public GameObject getLookObj()
        {
            return _lookObj;
        }

        public void faceDir(Vector2 dir)
        {
            if (_lookObj == null)
            {
                _lookObj = new GameObject();
                _lookObj.name = "lookObj";
                _lookObj.transform.parent = gameObject.transform;
            }
            _lookObj.transform.position = new Vector3(gameObject.transform.position.x + dir.x, gameObject.transform.position.y, gameObject.transform.position.z + dir.y);
            faceToNextBlock(_lookObj.transform);
        }
    }
}
