using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

namespace MTB
{
    public class MTBTaskConditionManager
    {
        private static string CONDITIONDATA_PATH = "Data/Plot/Task/Task_Condtion_Data";
        private static Dictionary<string, MTBTaskConditionData> TASKDATALIST = new Dictionary<string, MTBTaskConditionData>();

        private static MTBTaskConditionManager _instance;
        public static MTBTaskConditionManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBTaskConditionManager();
                return _instance;
            }
        }

        public void loadData()
        {
            TASKDATALIST.Clear();
            XmlDocument conditionData = new XmlDocument();
            conditionData.LoadXml(Resources.Load(CONDITIONDATA_PATH).ToString());
            XmlNodeList nodeList = conditionData.GetElementsByTagName("TaskCondition")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBTaskConditionData data = new MTBTaskConditionData();
                data.decode(xe);
                TASKDATALIST.Add(xe.GetAttribute("id"), data);
            }
        }

        public MTBTaskConditionData getData(string key)
        {
            if (!TASKDATALIST.ContainsKey(key))
                throw new Exception("没有配置taskcondition：" + key);
            return TASKDATALIST[key];
        }

        public void saveData(MTBTaskConditionData conditionData)
        {
            XmlDocument data = new XmlDocument();
            data.LoadXml(Resources.Load(CONDITIONDATA_PATH).ToString());
            XmlNode root = data.SelectSingleNode("TaskCondition");
            XmlElement element = conditionData.save(data);
            root.AppendChild(element);
            data.Save(Application.dataPath + "/Resources/" + CONDITIONDATA_PATH + ".xml");
        }
    }
}
