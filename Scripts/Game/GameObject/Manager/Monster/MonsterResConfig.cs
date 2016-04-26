using System.Collections.Generic;
namespace MTB
{
    public class MonsterResConfig
    {
        private static Dictionary<int, string> resConfig = new Dictionary<int, string>(){
			{1,"Prefabs/Monster/P_Monster_11_01"}   //随便写一个   后面有怪了改掉
         };

        public static string getResByType(int monsterId)
        {
            if (resConfig.ContainsKey(monsterId) == null)
            {
                return null;
            }
            return resConfig[monsterId];
        }
    }
}
