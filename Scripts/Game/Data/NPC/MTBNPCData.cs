using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class MTBNPCData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string resName { get; set; }
        public NPCType npcType { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = Convert.ToString(node.GetAttribute("name"));
            resName = Convert.ToString(node.GetAttribute("resName"));
            npcType = (NPCType)Convert.ToInt32(node.GetAttribute("npcType"));
        }

        public IData copy()
        {
            return new MTBNPCData();
        }
    }
}
