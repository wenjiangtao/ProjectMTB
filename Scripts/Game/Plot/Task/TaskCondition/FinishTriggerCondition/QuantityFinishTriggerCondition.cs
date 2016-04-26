using System;
using System.Collections.Generic;
namespace MTB
{
    public class QuantityFinishTriggerCondition : BaseTaskCondition
    {
        private int itemID;
        private Dictionary<int, int> item2QuatityMap = new Dictionary<int, int>(new QuantityConditionComparer());

        public override void setParams(Dictionary<string, string> paras)
        {
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
            return true;
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
            MTBArrowManager.Instance.disposeArrow();
            MTBTaskController.Instance.finishStep(taskId, stepId);
        }

        public override void dispose()
        {
            removeEvent();
            base.dispose();
        }
    }

    public class QuantityConditionComparer : IEqualityComparer<int>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<int>.Equals(int a, int b)
        {
            return a == b;
        }
        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return (int)obj;
        }
        #endregion
    }
}
