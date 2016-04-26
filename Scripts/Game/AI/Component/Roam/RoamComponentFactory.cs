using UnityEngine;
using System.Collections;
namespace MTB
{
    public class RoamComponentFactory
    {
        public static IRoamComponent creatRoamComponent(RoamComponentType type, GameObject host)
        {
            if (type == RoamComponentType.RANDOM_POINT)
                return new RandomPointRoamComponent(host);
            if (type == RoamComponentType.RANDOM_LODGED)
                return new LodgedRoamComponent(host);
            return null;
        }
    }
}
