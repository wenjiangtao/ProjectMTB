using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public enum UITypes
    {
        MAIN_UI = 1,
        MAIN_BAG = 2,
        CROSS = 3,
        SETUP = 4,
        START = 5,
        START_GAME = 6,
        LOADING = 7,
        SELECTWORLD = 8,
        DIALOG = 9,
        LOGIN = 10,
        REGISTER = 11,
        USERINFO = 12,
        PASSWORDFOUND = 13,
        COMIC = 14,
        TASK = 15
    }

    public class UIName
    {
        public static Dictionary<UITypes, string> Type2Name = new Dictionary<UITypes, string>()
        {
	        {UITypes.MAIN_UI,"MainUI"},
	        {UITypes.MAIN_BAG,"Bag"},
	        {UITypes.CROSS,"Cross"},
	        {UITypes.SETUP,"SetUp"},
	        {UITypes.START,"start"},
	        {UITypes.START_GAME,"StartGame"},
	        {UITypes.LOADING,"loading"},
	        {UITypes.SELECTWORLD,"SelectUI"},
	        {UITypes.DIALOG,"DialogPanel"},
			{UITypes.LOGIN,"Login"},
			{UITypes.REGISTER,"Register"},
			{UITypes.USERINFO,"UserInfo"},
			{UITypes.PASSWORDFOUND,"PasswordFound"},
            {UITypes.COMIC,"Comic"},
            {UITypes.TASK,"Task"}
        };
    }
}
