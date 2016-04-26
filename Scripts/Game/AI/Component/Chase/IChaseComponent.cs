using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IChaseComponent
    {
        /**
		 *  更新追踪目标 
		 * @param target
		 * 
		 */
        void updateTarget(GameObject target);

        /**
		 * 设置追踪移动类型  移动类型 MoveType.WALK  和 MoveTypes.JUMP
		 * @param type
		 * 
		 */
        void setMoveType(MoveType type);

        /**
          * 设置追踪参数，需要接近的范围  
          * @param minChase  最小靠近距离
          * @param maxChase  最大靠近距离
          * 
          */
        void setChaseParams(int minChase, int maxChase,int maxDistance);

        /**
		 *  追踪目标 
		 * @return 
		 * 
		 */
        bool onChasing();

        bool onLose();

        /**
		 * 是否接近目标 （在设置的靠近范围之内）
		 * @return 
		 * 
		 */
        bool isChaseTarget();

        void reset();

        void dispose();
    }
}
