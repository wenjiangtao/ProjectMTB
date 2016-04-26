using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    public class MTBNPCDataManager
    {
        private static string NPCDATA_PATH = "Data/NPC/NPCData";
        private static Dictionary<int, MTBNPCData> NPCDATALIST = new Dictionary<int, MTBNPCData>(new MTBNPCDataComparer());
        private static MTBNPCDataManager _instance;
        public static MTBNPCDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBNPCDataManager();
                return _instance;
            }
        }

        public MTBNPCData getData(int id)
        {
            if (!NPCDATALIST.ContainsKey(id))
            {
                throw new Exception("no NPCData Configure ,id:" + id);
            }
            return NPCDATALIST[id];
        }

        public void loadData()
        {
            XmlDocument npcData = new XmlDocument();
            npcData.LoadXml(Resources.Load(NPCDATA_PATH).ToString());
            XmlNodeList nodeList = npcData.GetElementsByTagName("NPCData")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBNPCData data = new MTBNPCData();
                data.decode(xe);
                NPCDATALIST.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }
    }

    public class MTBNPCDataComparer : IEqualityComparer<int>
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
