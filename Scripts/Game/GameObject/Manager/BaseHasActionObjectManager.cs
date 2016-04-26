/*********************************************************************************
 * 
 * manager类主要用于管理同类型gameobject
 * 维护gameobject列表方便外部获取和更新
 * 新增和移除gameobject需在对应manager中注册
 * 可直接在manager中移除gameobject
 * 
 * *******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class BaseHasActionObjectManager : IHasActionObjectManager
    {
        protected List<GameObject> _listObj;
        protected Dictionary<int, GameObject> _mapObj;
        protected Dictionary<WorldPos, List<GameObject>> _mapChunkPosObj;
        protected GameObject _parent;

        public BaseHasActionObjectManager(GameObject parent)
        {
            _parent = parent;
            _listObj = new List<GameObject>();
            _mapObj = new Dictionary<int, GameObject>(new HasActionObjectComparer());
            _mapChunkPosObj = new Dictionary<WorldPos, List<GameObject>>(new WorldPosComparer());
        }

        protected virtual void addObj(GameObject obj, GameObjectInfo info)
        {
            obj.transform.parent = _parent.transform;
            obj.GetComponent<BaseAttributes>().objectId = info.objectId;
            obj.GetComponent<BaseAttributes>().aoId = info.aoId;
            obj.GetComponent<BaseAttributes>().groupId = info.groupId;
            obj.GetComponent<BaseAttributes>().isNetObj = info.isNetObj;
            GameObjectController controller = obj.GetComponent<GameObjectController>();
            controller.InitControllerInfo();
            _listObj.Add(obj);
            _mapObj.Add(info.aoId, obj);

            //将对象添加到对应的区块位置中
            controller.On_ChangeChunk += HandleOn_ChangeChunk;
            List<GameObject> list;
            WorldPos pos = controller.gameObjectState.attachChunk.worldPos;
            _mapChunkPosObj.TryGetValue(pos, out list);
            if (list == null)
            {
                list = new List<GameObject>();
                _mapChunkPosObj.Add(pos, list);
            }
            list.Add(obj);
        }

        void HandleOn_ChangeChunk(GameObjectController controller, Chunk oldChunk, Chunk newChunk)
        {
            List<GameObject> list;
            _mapChunkPosObj.TryGetValue(oldChunk.worldPos, out list);
            if (list != null)
            {
                list.Remove(controller.gameObject);
                if (list.Count <= 0) _mapChunkPosObj.Remove(oldChunk.worldPos);
                list = null;
                _mapChunkPosObj.TryGetValue(newChunk.worldPos, out list);
                if (list == null)
                {
                    list = new List<GameObject>();
                    _mapChunkPosObj.Add(newChunk.worldPos, list);
                }
                list.Add(controller.gameObject);
            }
        }

        public List<GameObject> listObjInChunk(WorldPos chunkPos)
        {
            List<GameObject> list;
            _mapChunkPosObj.TryGetValue(chunkPos, out list);
            return list;
        }

        public void RemoveObjInChunk(WorldPos chunkPos)
        {
            List<GameObject> list;
            _mapChunkPosObj.TryGetValue(chunkPos, out list);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    removeObj(list[i].GetComponent<GameObjectController>().baseAttribute.aoId);
                }
            }
        }

        public List<GameObject> listObj()
        {
            return _listObj;
        }

        public virtual void removeObj(int aoId)
        {
            if (!_mapObj.ContainsKey(aoId))
            {
                return;
            }
            GameObject obj = _mapObj[aoId];
            removeObj(obj);
        }

        public virtual void RemoveObjBeside(int aoId)
        {
            List<int> removeAoIds = new List<int>();
            foreach (var item in _mapObj)
            {
                if (item.Key != aoId) removeAoIds.Add(item.Key);
            }
            for (int i = 0; i < removeAoIds.Count; i++)
            {
                removeObj(removeAoIds[i]);
            }
        }

        public virtual void removeObj(GameObject obj)
        {
            _listObj.Remove(obj);
            GameObjectController controller = obj.GetComponent<GameObjectController>();
            _mapObj.Remove(controller.baseAttribute.aoId);
            controller.On_ChangeChunk -= HandleOn_ChangeChunk;
            List<GameObject> list;
            WorldPos pos = controller.gameObjectState.attachChunk.worldPos;
            _mapChunkPosObj.TryGetValue(pos, out list);
            if (list != null)
            {
                list.Remove(obj);
                if (list.Count <= 0) _mapChunkPosObj.Remove(pos);
            }
            obj.SetActive(false);
            SelfDestroy.Destroy(obj);
        }

        public GameObject getObjById(int id)
        {
            if (_mapObj.ContainsKey(id))
                return _mapObj[id];
            else
                return null;
        }

        public virtual void objectDead(GameObject obj)
        {
            removeObj(obj);
        }

        public virtual void setObjectNameVisible(bool b)
        {
        }

        public virtual GameObject[] getOppositeGroupObj(int groupId)
        {
            return null;
        }

        public virtual void dispose() { }

    }



    public class HasActionObjectComparer : IEqualityComparer<int>
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
