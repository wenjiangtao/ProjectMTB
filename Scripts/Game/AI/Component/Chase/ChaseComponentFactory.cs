using UnityEngine;
using System.Collections;
namespace MTB
{
    public class ChaseComponentFactory
    {
        public static IChaseComponent createCharseComponent(ChaseComponentType type, GameObject host)
        {
            if (type == ChaseComponentType.SMART_CHASE)
                return new NormalChaseComponent(host);
            if (type == ChaseComponentType.RUNAWAY)
                return new RunAwayComponent(host);
            return null;
        }
    }
}
