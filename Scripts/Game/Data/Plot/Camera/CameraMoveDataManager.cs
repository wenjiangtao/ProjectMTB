using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    class CameraMoveDataManager
    {
        private static string CAMERAMOVEDATA_PATH = "Data/Plot/Camera/CameraMoveData";
        private static Dictionary<int, CameraMoveData> CAMERAMOVEDATALIST = new Dictionary<int, CameraMoveData>();

        private XmlDocument saveDocument;
        private static CameraMoveDataManager _instance;
        public static CameraMoveDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new CameraMoveDataManager();
                return _instance;
            }
        }

        public CameraMoveData getData(int id)
        {
            if (!CAMERAMOVEDATALIST.ContainsKey(id))
            {
                throw new Exception("no CameraMoveData Configure ,id:" + id);
            }
            return CAMERAMOVEDATALIST[id];
        }

        public int dataCount()
        {
            return CAMERAMOVEDATALIST.Count;
        }

        public void loadData()
        {
            CAMERAMOVEDATALIST.Clear();
            XmlDocument cameraData = new XmlDocument();
            cameraData.LoadXml(Resources.Load(CAMERAMOVEDATA_PATH).ToString());
            XmlNodeList nodeList = cameraData.GetElementsByTagName("CameraPath")[0].ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                CameraMoveData data = new CameraMoveData();
                data.decode(xe);
                CAMERAMOVEDATALIST.Add(Convert.ToInt32(xe.GetAttribute("id")), data);
            }
        }

        public void openDocument()
        {
            saveDocument = new XmlDocument();
            saveDocument.LoadXml(Resources.Load(CAMERAMOVEDATA_PATH).ToString());
        }

        public void addSaveData(CameraMoveData modeData)
        {
            if (saveDocument != null)
            {
                CAMERAMOVEDATALIST.Add(modeData.id, modeData);
                XmlNode root = saveDocument.SelectSingleNode("CameraPath");
                XmlElement element = modeData.save(saveDocument);
                root.AppendChild(element);
            }
        }

        public void removeData(int id)
        {
            if (saveDocument != null)
            {
                if (CAMERAMOVEDATALIST.ContainsKey(id))
                    CAMERAMOVEDATALIST.Remove(id);
                XmlNode root = saveDocument.SelectSingleNode("CameraPath");
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
            }
        }

        public int getInsertId()
        {
            int id = 1;

            for (int i = 0; i < CAMERAMOVEDATALIST.Keys.Count; i++)
            {
                if (!CAMERAMOVEDATALIST.ContainsKey(id))
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
            {
                XmlNode root = saveDocument.SelectSingleNode("CameraPath");
                root.RemoveAll();
                foreach (int key in CAMERAMOVEDATALIST.Keys)
                {
                    XmlElement element = CAMERAMOVEDATALIST[key].save(saveDocument);
                    root.AppendChild(element);
                }

                saveDocument.Save(Application.dataPath + "/Resources/" + CAMERAMOVEDATA_PATH + ".xml");
                saveDocument = null;
            }
        }

        public void dispose() { }
    }
}
