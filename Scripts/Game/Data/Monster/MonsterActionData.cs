using UnityEngine;
using System;
using System.Xml;
using System.Collections;
namespace MTB
{
    public class MonsterActionData : IData
    {
        public int id { get; set; }
        public int defaultAction { get; set; }
        public int attackAction { get; set; }
        public int[] specialAction { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            defaultAction = Convert.ToInt32(node.GetAttribute("default"));
            attackAction = Convert.ToInt32(node.GetAttribute("attack"));
            string[] actionStrs = Convert.ToString(node.GetAttribute("special")).Split(',');
            specialAction = new int[actionStrs.Length];
            for (int i = 0; i < actionStrs.Length; i++)
            {
                specialAction[i] = Convert.ToInt32(actionStrs[i]);
            }
        }

        public IData copy()
        {
            return new MonsterActionData();
        }
    }
}
