using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class AIComponentHelper
    {
        private static System.Random _random = new System.Random();

        public static Vector3 getRandomPoint(Vector3 centerpoint, int radius, int minDistance = 0, int checkTimes = 5)
        {
            Vector3 result = centerpoint;
            int scaleX = _random.Next(100) > 50 ? 1 : -1;
            int scaleZ = _random.Next(100) > 50 ? 1 : -1;
            while (checkTimes > 0)
            {
                Vector3 point = tryGetRandomPoint(centerpoint, radius, minDistance, scaleX, scaleZ);
                if (point.y != -1000)
                {
                    return point;
                }
                checkTimes--;
            }
            return result;
        }

        private static Vector3 tryGetRandomPoint(Vector3 centerpoint, int radius, int minDistance, int scaleX, int scaleZ)
        {
            int randomDistX = _random.Next(radius);
            int randomDistZ = _random.Next(radius);
            if (randomDistX * randomDistZ < minDistance * minDistance)
            {
                if (_random.Next(100) > 50)
                {
                    randomDistX += minDistance / 2;
                }
                else
                {
                    randomDistZ += minDistance / 2;
                }
            }

            int desX = Convert.ToInt32(centerpoint.x) + randomDistX * scaleX;
            int desZ = Convert.ToInt32(centerpoint.z) + randomDistZ * scaleZ;

            return MTBPathFinder.Instance.getCanWalkPointByXZ(desX, Convert.ToInt32(centerpoint.y), desZ, radius);
        }
    }
}
