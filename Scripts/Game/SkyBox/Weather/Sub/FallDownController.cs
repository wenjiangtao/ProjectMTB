using UnityEngine;
using System.Collections;
namespace MTB
{
    class FallDownController : IWeatherController
    {
        protected GameObject[] _directObjs;
        protected GameObject _weatherObj;
        protected Transform _playerTrans;
        private int _updateCount;

        public FallDownController(GameObject weatherObj)
        {
            this._weatherObj = weatherObj;
            init();
            initDirObj();
        }

        protected virtual void init()
        {
            _playerTrans = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform;
            _updateCount = 0;
        }

        protected virtual void initDirObj()
        {
            _directObjs = new GameObject[9];
        }

        public void updateView()
        {
            _updateCount = 0;
            for (int z = (int)_playerTrans.position.z - 1; z < (int)_playerTrans.position.z + 2; z++)
            {
                for (int x = (int)_playerTrans.position.x - 1; x < (int)_playerTrans.position.x + 2; x++)
                {
                    _directObjs[_updateCount].SetActive(true);
                    for (int y = (int)(_playerTrans.position.y) + 1; y < WorldConfig.Instance.heightCap; y++)
                    {
                        if (World.world.GetBlock(x, y, z).BlockType != BlockType.Air)
                        {
                            _directObjs[_updateCount].SetActive(false);
                            y = WorldConfig.Instance.heightCap;
                        }
                    }
                    _updateCount++;
                }
            }
        }

        public void setEnable(bool b)
        {
            _weatherObj.SetActive(b);
        }

        public void setParent(Transform p)
        {
            _weatherObj.transform.parent = p;
        }
        public void setPosition(Vector3 p)
        {
            _weatherObj.transform.position = p;
        }
        public void dispose()
        {
        }
    }
}
