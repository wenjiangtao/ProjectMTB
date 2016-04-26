using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IRoamComponent
    {
        /**
         * 设置移动类型   可能是walk或者Jump  
         * 
         * **/
        void setMoveType(MoveType type);

        /**
         * 设置漫游中心点
         * 
         * **/
        void setRoamCenterPoint(Vector3 point);

        /**
         * 设置漫游参数
         * 
         * **/
        void setRoamParams(int radius, int idleTime, int minRoamDis);

        /**
         * 设置当前漫游状态
         * 
         * **/
        void setRoamType(RoamType type);

        /**
         * 获取当前漫游状态
         * 
         * **/
        RoamType getRoamType();

        /**
         * 获得当前朝向角度
         * 
         * **/
        float getFaceDirection();

        /**
         * 漫游（每帧执行）
         * 
         * **/
        void onRoam();


        void reset();

        void dispose();
    }
}
