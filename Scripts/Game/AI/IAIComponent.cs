using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IAIComponent
    {
        /**
             *  绑定战斗物件
             * @param fightGameObject
             *
             */
        void attach(GameObject haveActionObj);
        /**
             *  当前AI是否正在运行
             * @return
             *
             */
        bool isRunning();
        /**
             *  暂定当前AI
             *
             */
        void pause();
        /**
             *  开始运行AI
             *
             */
        void run();
        /**
             *  获取当前AI宿主
             *
             */
        GameObject host();
        /**
             *  设置AI目标
             *
             */
        void setTarget(GameObject value);
        /**
             *  获取AI目标
             *
             */
        GameObject getTarget();
        /**
             * 获取AI数据
             * 
             */
        IData getAIDate();
        /**
             *  每帧执行的
             *
             */

        void onTick();
        /**
             *  攻击目标列表
             *
             */
        GameObject[] listArrTarget();
        /**
             *  攻击目标类型
             *
             */
        void setAttTargetType(GameObjectTypes value);
        /**
             *  获取当前最大仇恨值
             *
             */
        int getMaxEnmity();
        /**
             *  更新单个目标仇恨值
             *
             */
        void updateEnmityByFoId(int aoId, int enmity);
        /**
             *  更新某个阵营仇恨值
             *
             */
        void updateEnmityByGroupId(int groupId, int enmity);
        /**
             * 搜寻攻击目标
             * 
             */
        GameObject seachTarget(bool checkHeight = true);
        /**
             * 判断是否为可攻击类型
             * 
             */
        bool isAttachTargetType(int type);

        void dispose();
    }
}
