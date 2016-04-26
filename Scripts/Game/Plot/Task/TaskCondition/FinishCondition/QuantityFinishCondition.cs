using System;
using System.Collections.Generic;
namespace MTB
{
    public class QuantityFinishCondition : BaseTaskCondition
    {
        private int itemID;
        private UserItem conditionItem;
        private Dictionary<int, int> item2QuatityMap = new Dictionary<int, int>(new QuantityConditionComparer());
        private bool conditionMark;

        public override void setParams(Dictionary<string, string> paras)
        {
            conditionMark = false;
            string paraStr;
            string[] paraList;
            paras.TryGetValue("params", out paraStr);
            paraList = paraStr.Split(',');
            for (int i = 0; i < paraList.Length; i++)
            {
                string[] itemParaList = paraList[i].Split(':');
                item2QuatityMap.Add(Convert.ToInt32(itemParaList[0]), Convert.ToInt32(itemParaList[1]));
            }
            base.setParams(paras);
            InitEvent();
        }

        public override bool MeetCondition()
        {
            return conditionMark;
        }

        private void InitEvent()
        {
            BackpackItemManager.Instance.OnUserItemAdd += onUserItemAdd;
            BackpackItemManager.Instance.OnUserItemPropertyChanged += OnUserItemPropertyChanged;
        }

        private void removeEvent()
        {
            BackpackItemManager.Instance.OnUserItemAdd -= onUserItemAdd;
            BackpackItemManager.Instance.OnUserItemPropertyChanged -= OnUserItemPropertyChanged;
        }

        private void onUserItemAdd(UserItem userItem)
        {
            if (item2QuatityMap.ContainsKey(userItem.id))
            {
                foreach (int key in item2QuatityMap.Keys)
                {
                    if (BackpackItemManager.Instance.GetUserItem(key).num <= item2QuatityMap[key]) return;
                }
                onConditionFinish();
            }
        }

        private void OnUserItemPropertyChanged(UserItem userItem, string property, object oldValue, object newValue)
        {
            if (item2QuatityMap.ContainsKey(userItem.id))
            {
                foreach (int key in item2QuatityMap.Keys)
                {
                    if (BackpackItemManager.Instance.GetUserItem(key).num <= item2QuatityMap[key]) return;
                }
                onConditionFinish();
            }
        }

        private void onConditionFinish()
        {
            removeEvent();
            conditionMark = true;
        }
    }
}
