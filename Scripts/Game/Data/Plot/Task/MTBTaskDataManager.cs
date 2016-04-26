using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

namespace MTB
{
    public class MTBTaskDataManager
    {
        private static string TASKDATA_PATH = "Data/Plot/Task/TaskData";
        private static Dictionary<int, MTBTaskData> TASKDATALIST = new Dictionary<int, MTBTaskData>(new MTBTaskDataComparer());
        private static MTBTaskDataManager _instance;
        public static MTBTaskDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBTaskDataManager();
                return _instance;
            }
        }

        public MTBTaskData getData(int id)
        {
            if (!TASKDATALIST.ContainsKey(id))
            {
                throw new Exception("no TaskData Configure ,id:" + id);
            }
            return TASKDATALIST[id];
        }

        public void loadData()
        {
            TASKDATALIST.Clear();
            MTBDialogueDataManager.Instance.loadData();
            MTBTaskConditionManager.Instance.loadData();
            MTBArrowDataManager.Instance.loadData();
            NPCPosDataManager.Instance.loadData();
            XmlDocument dialogueData = new XmlDocument();
            dialogueData.LoadXml(Resources.Load(TASKDATA_PATH).ToString());
            XmlNodeList nodeList = dialogueData.GetElementsByTagName("Task")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBTaskData data = new MTBTaskData();
                data.decode(xe);
                TASKDATALIST.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
            CameraMoveDataManager.Instance.loadData();
        }

        public void dispose() { }
    }

    public class MTBTaskDataComparer : IEqualityComparer<int>
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
