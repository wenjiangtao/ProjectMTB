using UnityEngine;
using System;
using System.Xml;
namespace MTB
{
    public class NPCPosData : IData
    {
        public int id;
        public int npcId;
        public string doc;
        public Vector3 pos;

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            npcId = Convert.ToInt32(node.GetAttribute("npcId"));
            doc = Convert.ToString(node.GetAttribute("doc"));
            string[] posStrList = Convert.ToString(node.GetAttribute("pos")).Split(',');
            pos = new Vector3(Convert.ToSingle(posStrList[0]), Convert.ToSingle(posStrList[1]), Convert.ToSingle(posStrList[2]));
        }

        public XmlElement save(XmlDocument xml)
        {
            XmlElement posNode = xml.CreateElement("PosData");
            posNode.SetAttribute("id", id.ToString());
            posNode.SetAttribute("npcId", npcId.ToString());
            posNode.SetAttribute("doc", doc);
            posNode.SetAttribute("pos", pos.x + "," + pos.y + "," + pos.z);
            return posNode;
        }

        public IData copy()
        {
            return new NPCPosData();
        }
    }
}
