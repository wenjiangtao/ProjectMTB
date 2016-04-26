using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class PlayerManager : BaseHasActionObjectManager
    {
        private GameObject _mainPlayer;

        public PlayerManager(GameObject parent)
            : base(parent)
        {
        }

        public void InitPlayer(PlayerInfo info)
        {
            string resPath = PlayerResConfig.getResByType(info.playerId);
            if (resPath == null)
            {
                Debug.LogError("当前playertype有误或者没有配置对应资源");
                return;
            }
            GameObject player = GameObject.Instantiate(Resources.Load(resPath) as GameObject);
            player.transform.position = info.position;
            if (info.isMe)
			{
				_mainPlayer = player;
				_mainPlayer.transform.localRotation = Quaternion.AngleAxis(WorldConfig.Instance.birthRotateY,Vector3.up);
			}

            addObj(player, info);
        }

        public GameObject getMyPlayer()
        {
            return _mainPlayer;
        }

        public override void removeObj(GameObject obj)
        {
            base.removeObj(obj);
            GameObjectController controller = obj.GetComponent<GameObjectController>();
            GameObjectController mainController = _mainPlayer.GetComponent<GameObjectController>();
            if (controller.baseAttribute.aoId == mainController.baseAttribute.aoId) _mainPlayer = null;
        }

        public GameObject[] getListPlayer()
        {
            return _listObj.ToArray();
        }
    }

    public class PlayerComparer : IEqualityComparer<int>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<int>.Equals(int a, int b)
        {
            return a == b;
        }
        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return (int)obj;
        }
        #endregion
    }

}
