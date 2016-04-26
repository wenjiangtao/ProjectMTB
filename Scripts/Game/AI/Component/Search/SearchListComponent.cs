using UnityEngine;
using System.Collections;
namespace MTB
{
    public class SearchListComponent : ISearchComponent
    {
        private GameObject _host;

        public SearchListComponent(GameObject host)
        {
            this._host = host;
        }

        public GameObject searchTarget(int radius,int mindis, int groupId, bool checkHeight = true)
        {
            Debug.LogError("SearchMonsterComponent暂时不支持这个功能!");
            return null;
        }

        public GameObject searchTargetByList(GameObject[] listTarget, int mindis, int radius, bool checkHeight = true)
        {
            GameObject result = null;
            float minDis = float.MaxValue;

            foreach (GameObject target in listTarget)
            {
                float distance = Vector3.Distance(target.transform.position, _host.transform.position);

                if (minDis > distance)
                {
                    minDis = distance;
                    result = target;
                }
            }
            if (minDis <= radius && minDis > 1)
                return result;
            Debug.LogError("搜索不到目标，长度为：" + listTarget.Length);
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
