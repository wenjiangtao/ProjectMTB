using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface ISearchComponent
    {
        /**
		 *  查找目标
		 *  @checkHeight  是否进行高度判断
		 */
        GameObject searchTarget(int radius,int mindis, int groupId, bool checkHeight = true);
        /**
		 * 根据目标数组查找目标
		 * @param listTargets
		 * @param radius
		 * @param checkHeight
		 * @return 
		 * 
		 */
        GameObject searchTargetByList(GameObject[] listTarget, int radius, int mindis,bool checkHeight = true);
        /**
         *  重置 
         */
        void reset();

        void dispose();
    }
}
