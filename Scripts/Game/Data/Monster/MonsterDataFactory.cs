using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class MonsterDataFactory
    {
        private static Dictionary<int, IData> BASICDATA = new Dictionary<int, IData>();
        private static Dictionary<int, IData> AIDATA = new Dictionary<int, IData>();
        private static Dictionary<int, IData> ACTIONDATA = new Dictionary<int, IData>();
        private static Dictionary<int, IData> BREEDDATA = new Dictionary<int, IData>();

        public static IData getData(int id, DataTypes type)
        {
            if (type == DataTypes.Basic)
                return BASICDATA[id];
            if (type == DataTypes.AI)
                return AIDATA[id];
            if (type == DataTypes.Action)
                return ACTIONDATA[id];
            if (type == DataTypes.Breeding)
                return BREEDDATA[id];
            Debug.LogError("没有此类型数据");
            return null;
        }

        public static void SaveBasicData(XmlDocument xdoc)
        {
            XmlNodeList nodeList = xdoc.GetElementsByTagName("MonsterBatch")[0].ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                MonsterBasicData data = new MonsterBasicData();
                data.decode(xe);
                BASICDATA.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public static void SaveAIData(XmlDocument xdoc)
        {
            XmlNodeList nodeList = xdoc.GetElementsByTagName("MonsterBatch")[0].ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                MonsterAIData data = new MonsterAIData();
                data.decode(xe);
                AIDATA.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public static void SaveActionData(XmlDocument xdoc)
        {
            XmlNodeList nodeList = xdoc.GetElementsByTagName("MonsterBatch")[0].ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                MonsterActionData data = new MonsterActionData();
                data.decode(xe);
                ACTIONDATA.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public static void SaveBreedData(XmlDocument xdoc)
        {
            XmlNodeList nodeList = xdoc.GetElementsByTagName("MonsterBatch")[0].ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                MonsterBreedData data = new MonsterBreedData();
                data.decode(xe);
                BREEDDATA.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public static void dispose()
        {
            BASICDATA = null;
            AIDATA = null;
            ACTIONDATA = null;
            BREEDDATA = null;
        }
    }
}
