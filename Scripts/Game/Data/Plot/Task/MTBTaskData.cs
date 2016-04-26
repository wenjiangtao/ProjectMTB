using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class MTBTaskData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string[] preTaskList { get; set; }
        public string[] openTaskList { get; set; }
        public Dictionary<int, MTBTaskStepData> stepList { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = Convert.ToString(node.GetAttribute("name"));
            string preTaskStr = Convert.ToString(node.GetAttribute("pretask"));
            preTaskList = preTaskStr.Split(',');
            string openTaskStr = Convert.ToString(node.GetAttribute("opentask"));
            openTaskList = openTaskStr.Split(',');
            XmlNodeList nodeList = node.GetElementsByTagName("Step");
            stepList = new Dictionary<int, MTBTaskStepData>();
            foreach (XmlElement item in nodeList)
            {
                MTBTaskStepData data = new MTBTaskStepData();
                data.decode(item);
                stepList.Add(data.id, data);
            }
        }
        public IData copy()
        {
            return new MTBTaskData();
        }
    }

    public class MTBTaskStepData : IData
    {
        public int id { get; set; }
        public string name { get; set; }

        public int cameraMoveId { get; set; }
        public int dialogId { get; set; }

        public string nextstep { get; set; }

        public string condtion { get; set; }

        public NPCPosData npcPosData { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = node.GetAttribute("name");
            cameraMoveId = Convert.ToInt32(node.GetAttribute("cameraMoveId"));
            dialogId = Convert.ToInt32(node.GetAttribute("dialogId"));
            nextstep = node.GetAttribute("nextstep");
            condtion = node.GetAttribute("condition");
            npcPosData = NPCPosDataManager.Instance.getData(Convert.ToInt32(node.GetAttribute("npcposdata")));
        }

        public IData copy()
        {
            return new MTBTaskStepData();
        }

    }
}
