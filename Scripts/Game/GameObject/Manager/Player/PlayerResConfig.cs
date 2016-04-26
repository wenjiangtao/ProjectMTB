using System.Collections.Generic;
namespace MTB
{
    public class PlayerResConfig
    {
        private static Dictionary<int, string> resConfig = new Dictionary<int, string>(){
             {1,"Prefabs/Player"}
         };

        public static string getResByType(int playerId)
        {
            if (resConfig.ContainsKey(playerId) == null)
            {
                return null;
            }
            return resConfig[playerId];
        }
    }
}
