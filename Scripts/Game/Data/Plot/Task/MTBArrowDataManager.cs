using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

namespace MTB
{
    public class MTBArrowDataManager
    {
        private static string ARROWDATA_PATH = "Data/Plot/Task/ArrowData";

        private static Dictionary<string, MTBArrowData> ARROWDATALIST = new Dictionary<string, MTBArrowData>(new MTBARROWDataComparer());
        private XmlDocument saveDocument;

        private static MTBArrowDataManager _instance;
        public static MTBArrowDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MTBArrowDataManager();
                return _instance;
            }
        }

        public MTBArrowData getData(string id)
        {
            if (!ARROWDATALIST.ContainsKey(id))
            {
                return null;
            }
            return ARROWDATALIST[id];
        }

        public void loadData()
        {
            ARROWDATALIST.Clear();
            XmlDocument arrowData = new XmlDocument();
            arrowData.LoadXml(Resources.Load(ARROWDATA_PATH).ToString());
            XmlNodeList nodeList = arrowData.GetElementsByTagName("Arrow")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                MTBArrowData data = new MTBArrowData();
                data.decode(xe);
                ARROWDATALIST.Add(xe.GetAttribute("id"), data);
            }
        }

        public int dataCount()
        {
            return ARROWDATALIST.Count;
        }

        public void openDocument()
        {
            saveDocument = new XmlDocument();
            saveDocument.LoadXml(Resources.Load(ARROWDATA_PATH).ToString());
        }

        public void addSaveData(MTBArrowData modeData)
        {
            if (!checkIdCanUse(modeData.id))
                return;
            if (saveDocument != null)
            {
                XmlNode root = saveDocument.SelectSingleNode("Arrow");
                XmlElement element = modeData.save(saveDocument);
                root.AppendChild(element);
                ARROWDATALIST.Add(modeData.id, modeData);
                saveDocument.Save(Application.dataPath + "/Resources/" + ARROWDATA_PATH + ".xml");
            }
        }

        public void removeData(string id)
        {
            if (saveDocument != null)
            {
                if (ARROWDATALIST.ContainsKey(id))
                    ARROWDATALIST.Remove(id);
                XmlNode root = saveDocument.SelectSingleNode("Arrow");
                XmlElement removeEle = null;
                foreach (XmlElement ele in root.ChildNodes)
                {
                    if (id.Equals(ele.GetAttribute("id")))
                    {
                        removeEle = ele;
                    }
                }
                if (removeEle != null)
                    root.RemoveChild(removeEle);
                saveDocument.Save(Application.dataPath + "/Resources/" + ARROWDATA_PATH + ".xml");
            }
        }

        //编辑模式使用
        public void saveData()
        {
            if (saveDocument != null)
                saveDocument.Save(Application.dataPath + "/Resources/" + ARROWDATA_PATH + ".xml");
            saveDocument = null;
        }

        public bool checkIdCanUse(string id)
        {
            return !ARROWDATALIST.ContainsKey(id);
        }

        public void dispose() { }
    }

    public class MTBARROWDataComparer : IEqualityComparer<string>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<string>.Equals(string a, string b)
        {
            return a.Equals(b);
        }
        int IEqualityComparer<string>.GetHashCode(string obj)
        {
            return obj.ToString().GetHashCode();
        }
        #endregion
    }
}
