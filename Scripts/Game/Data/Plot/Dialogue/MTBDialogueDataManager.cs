/*****
 * 只保存当前task对应的dialog文件
 * *****/
using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

namespace MTB
{
    public class MTBDialogueDataManager
    {
        private static string DIALOGUEDATA_PATH = "Data/Plot/Task/Dialogue_Data";
        private static Dictionary<int, MTBTaskDialogueData> DIALOGUEDATALIST = new Dictionary<int, MTBTaskDialogueData>();
        public int DialogueId { get; set; }
        private static MTBDialogueDataManager _instance;
        public static MTBDialogueDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBDialogueDataManager();
                return _instance;
            }
        }

        public MTBDialogueData getData(int taskId, int id)
        {
            if (!DIALOGUEDATALIST.ContainsKey(taskId))
                throw new Exception("no TaskDialogueData Configure ,id:" + taskId);
            if (!DIALOGUEDATALIST[taskId].dialogueDataList.ContainsKey(id))
                throw new Exception("no DialogueData Configure ,id:" + id);
            return DIALOGUEDATALIST[taskId].dialogueDataList[id];
        }

        public void loadData()
        {
            DIALOGUEDATALIST.Clear();
            XmlDocument dialogueData = new XmlDocument();
            dialogueData.LoadXml(Resources.Load(DIALOGUEDATA_PATH).ToString());
            XmlNodeList nodeList = dialogueData.GetElementsByTagName("DialogueConfig")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBTaskDialogueData data = new MTBTaskDialogueData();
                data.decode(xe);
                DIALOGUEDATALIST.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public bool hasData(int dialogueId)
        {
            return DialogueId == dialogueId;
        }

        public void dispose() { }
    }
}
