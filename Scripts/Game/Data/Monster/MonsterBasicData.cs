using UnityEngine;
using System;
using System.Collections;
using System.Xml;
namespace MTB
{
    public class MonsterBasicData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string respath { get; set; }
        public int aiId { get; set; }
        public int aiComponent { get; set; }
        public int actionId { get; set; }
        public int breedId { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = Convert.ToString(node.GetAttribute("name"));
            respath = Convert.ToString(node.GetAttribute("respath"));
            aiId = Convert.ToInt32(node.GetAttribute("aiid"));
            aiComponent = Convert.ToInt32(node.GetAttribute("aicomponent"));
            actionId = Convert.ToInt32(node.GetAttribute("actionid"));
            breedId = Convert.ToInt32(node.GetAttribute("breedId"));
        }

        public IData copy()
        {
            return new MonsterBasicData();
        }
    }
}
