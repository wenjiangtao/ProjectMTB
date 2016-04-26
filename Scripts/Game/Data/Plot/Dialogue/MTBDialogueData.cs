using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class MTBTaskDialogueData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public Dictionary<int, MTBDialogueData> dialogueDataList { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = node.GetAttribute("name");
            XmlNodeList nodeList = node.GetElementsByTagName("Dialogue");
            dialogueDataList = new Dictionary<int, MTBDialogueData>();
            foreach (XmlElement item in nodeList)
            {
                MTBDialogueData data = new MTBDialogueData();
                data.decode(item);
                dialogueDataList.Add(data.id, data);
            }
        }

        public IData copy()
        {
            return new MTBTaskDialogueData();
        }
    }

    public class MTBDialogueData : IData
    {
        public int id { get; set; }
        public Dictionary<int, MTBDialogueStepData> dialogueList { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            XmlNodeList nodeList = node.GetElementsByTagName("Step");
            dialogueList = new Dictionary<int, MTBDialogueStepData>();
            foreach (XmlElement item in nodeList)
            {
                MTBDialogueStepData data = new MTBDialogueStepData();
                data.decode(item);
                dialogueList.Add(data.id, data);
            }
        }

        public IData copy()
        {
            return new MTBDialogueData();
        }
    }

    public class MTBDialogueStepData : IData
    {
        public int id { get; set; }
        public int npcId { get; set; }
        public string content { get; set; }
        //next = end表示这是最后一段对话
        public string next { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            npcId = Convert.ToInt32(node.GetAttribute("npc"));
            content = Convert.ToString(node.GetAttribute("content"));
            next = Convert.ToString(node.GetAttribute("nextstep"));
        }

        public IData copy()
        {
            return new MTBDialogueStepData();
        }
    }
}
