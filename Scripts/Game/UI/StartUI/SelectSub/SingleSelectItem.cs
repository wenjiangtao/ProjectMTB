using System;
namespace MTB
{
    public class SingleSelectItem : SelectItem
    {
        private string worldConfigStr;
        private string worldName;
        private int seed;

        public override void Init(string id, params object[] paras)
        {
            base.Init(id, paras);
            worldConfigStr = (string)paras[1];
            worldName = (string)paras[2];
            string lastSaveTimeStr = (string)paras[3];
            seed = (int)paras[4];
            string worldNameStr = worldName.Split('-')[0];
            string[] timeInfos = lastSaveTimeStr.Split('-');
            string showInfo;
            if (timeInfos.Length > 1)
                showInfo = worldNameStr + "         " + timeInfos[0] + "年" + timeInfos[1] + "月" + timeInfos[2] + "日" + timeInfos[3] + "时" + timeInfos[4] + "分" + timeInfos[5] + "秒最后修改";
            else
                showInfo = worldNameStr;
            _text.text = showInfo;
        }

        protected override void OnSelect()
        {
            EventManager.SendEvent(UIEventMacro.CLICK_ITEM, UItype, NetType.Single, worldConfigStr, worldName, seed);
        }
    }
}

