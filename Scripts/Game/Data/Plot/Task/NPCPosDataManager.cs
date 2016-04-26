using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

namespace MTB
{
    public class NPCPosDataManager
    {
        private static string NPCPOSDATA_PATH = "Data/Plot/Task/NpcPosData";
        private static Dictionary<int, NPCPosData> NPCDATALIST = new Dictionary<int, NPCPosData>(new MTBNPCPosDataComparer());
        private XmlDocument saveDocument;

        private static NPCPosDataManager _instance;
        public static NPCPosDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new NPCPosDataManager();
                return _instance;
            }
        }

        public NPCPosData getData(int id)
        {
            if (!NPCDATALIST.ContainsKey(id))
            {
                if (id != 0)
                    throw new Exception("no TaskData Configure ,id:" + id);
                return null;
            }
            return NPCDATALIST[id];
        }

        public void loadData()
        {
            NPCDATALIST.Clear();
            XmlDocument posData = new XmlDocument();
            posData.LoadXml(Resources.Load(NPCPOSDATA_PATH).ToString());
            XmlNodeList nodeList = posData.GetElementsByTagName("NPCPosition")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                NPCPosData data = new NPCPosData();
                data.decode(xe);
                NPCDATALIST.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }


        public int dataCount()
        {
            return NPCDATALIST.Count;
        }

        public void openDocument()
        {
            saveDocument = new XmlDocument();
            saveDocument.LoadXml(Resources.Load(NPCPOSDATA_PATH).ToString());
        }

        public void addSaveData(NPCPosData modeData)
        {
            if (saveDocument != null)
            {
                XmlNode root = saveDocument.SelectSingleNode("NPCPosition");
                XmlElement element = modeData.save(saveDocument);
                root.AppendChild(element);
                NPCDATALIST.Add(modeData.id, modeData);
                saveDocument.Save(Application.dataPath + "/Resources/" + NPCPOSDATA_PATH + ".xml");
            }
        }

        public void removeData(int id)
        {
            if (saveDocument != null)
            {
                if (NPCDATALIST.ContainsKey(id))
                    NPCDATALIST.Remove(id);
                XmlNode root = saveDocument.SelectSingleNode("NPCPosition");
                XmlElement removeEle = null;
                foreach (XmlElement ele in root.ChildNodes)
                {
                    if (id == Convert.ToInt32(ele.GetAttribute("id")))
                    {
                        removeEle = ele;
                    }
                }
                if (removeEle != null)
                    root.RemoveChild(removeEle);
                saveDocument.Save(Application.dataPath + "/Resources/" + NPCPOSDATA_PATH + ".xml");
            }
        }

        public int getInsertId()
        {
            int id = 1;

            for (int i = 0; i < NPCDATALIST.Keys.Count; i++)
            {
                if (!NPCDATALIST.ContainsKey(id))
                {
                    return id;
                }
                id++;
            }
            id = dataCount() + 1;
            return id;
        }

        //编辑模式使用
        public void saveData()
        {
            if (saveDocument != null)
                saveDocument.Save(Application.dataPath + "/Resources/" + NPCPOSDATA_PATH + ".xml");
            saveDocument = null;
        }

        public void dispose() { }
    }

    public class MTBNPCPosDataComparer : IEqualityComparer<int>
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
