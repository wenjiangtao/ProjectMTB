using UnityEngine;
using System.Collections;
namespace MTB
{
    public class MonsterAIFactory
    {

        public static IAIComponent getAIComponent(int aiComponentId)
        {
            if (aiComponentId == 2)
            {
                return new BatAIComponent();
            }
            if (aiComponentId == 3)
            {
                return new LodgedAIComponent();
            }
            if (aiComponentId == 4)
            {
                return new NormalHabitAIComponent();
            }
            else
            {
                return new MonsterAIComponent();
            }
        }
    }
}
