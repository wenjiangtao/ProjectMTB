using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class SearchMonsterComponent : ISearchComponent
    {
        private GameObject _host;
        public SearchMonsterComponent(GameObject host)
        {
            this._host = host;
        }


        public GameObject searchTarget(int radius,int mindis, int groupId, bool checkHeight = true)
        {
            GameObject target = null;
            float minDis = float.MaxValue;
			List<GameObject> list = HasActionObjectManager.Instance.getManager(HasActionObjectManagerTypes.MONSTER).listObj();
			for (int i = 0; i < list.Count; i++) {
				float distance = Vector3.Distance(list[i].transform.position, _host.transform.position);
				
				if (minDis > distance)
				{
					minDis = distance;
					target = list[i];
				}
			}

            if (minDis <= radius && minDis > mindis)
                return target;
            return null;
        }

        public GameObject searchTargetByList(GameObject[] listTarget, int mindis, int radius, bool checkHeight = true)
        {
            Debug.LogError("SearchMonsterComponent暂时不支持这个功能!");
            return null;
        }

        public void reset()
        {

        }

        public void dispose()
        {
            this._host = null;
        }
    }
}
