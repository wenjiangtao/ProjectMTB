using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    public class MTBPlantDataManager
    {
        private static string PLANTDATA_PATH = "Data/Plant/PlantData";
        private static Dictionary<int, MTBPlantData> PLANTDATALIST = new Dictionary<int, MTBPlantData>(new MTBPlantDataComparer());
        private static MTBPlantDataManager _instance;
        public static MTBPlantDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBPlantDataManager();
                return _instance;
            }
        }

        public MTBPlantData getData(int id)
        {
            if (!PLANTDATALIST.ContainsKey(id))
            {
                throw new Exception("no DialogueData Configure ,id:" + id);
            }
            return PLANTDATALIST[id];
        }

        public void loadData()
        {
            XmlDocument dialogueData = new XmlDocument();
            dialogueData.LoadXml(Resources.Load(PLANTDATA_PATH).ToString());
            XmlNodeList nodeList = dialogueData.GetElementsByTagName("PlantData")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBPlantData data = new MTBPlantData();
                data.decode(xe);
                PLANTDATALIST.Add(Convert.ToInt32(xe.GetAttribute("decorationType")), data);
                if (data.seedId != 0)
                    PlantConfig.SeedlingList.Add(data.seedId, (DecorationType)data.decorationType);
            }
        }

        public void dispose() { }
    }

    public class MTBPlantDataComparer : IEqualityComparer<int>
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
