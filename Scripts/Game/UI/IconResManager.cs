using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class IconResManager
    {
        private static Dictionary<int, string> dic = new Dictionary<int, string>()
    {
        {1,"Tex_Block_01_up"},
        {2,"Tex_Block_02_up"},
        {3,"Tex_Block_03_up"},
        {12,"Tex_Block_12_up"},
        {14,"Tex_Block_14_up"},
        {18,"Tex_Block_18_up"},
		{38,"Tex_Block_38_up"},
        {40,"Tex_Block_40_up_down"},
        {41,"Tex_Block_41_up"},
        {42,"Tex_Block_42_all"},
        {43,"Tex_Block_43_all"},
        {44,"Tex_Block_44_up"},
        {45,"Tex_Block_45_up"},
        {46,"Tex_Block_46_all"},
        {48,"Tex_Block_48_all"},
        {49,"Tex_Block_49_all"},
        {50,"Tex_Block_50_up"},
		{100,"Icon_item_1001"},
        {1001,"Icon_item_1001"},
        {1002,"Icon_item_1002"},
        {1003,"Icon_item_1003"},
        {1004,"Icon_item_1004"},
        {1005,"Icon_item_1005"},
        {1006,"Icon_item_1006"},
        {1007,"Icon_item_1007"},
        {1008,"Icon_item_1008"},
        {1009,"Icon_item_1009"},
        {1010,"Icon_item_1010"},
        {1011,"Icon_item_1011"},
        {1012,"Icon_item_1012"}
     };

        public static string getIconNameByMId(int mId)
        {
			return "Icon_Item_" + mId;
//            if (dic.ContainsKey(mId))
//            {
//                return dic[mId];
//            }
//            return dic[1];
        }


    }
}
