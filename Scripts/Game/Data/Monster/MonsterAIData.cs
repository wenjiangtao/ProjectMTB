using UnityEngine;
using System;
using System.Xml;
using System.Collections;
namespace MTB
{
    public class MonsterAIData : IData
    {
        public int id { get; set; }
        public float moveSpeed { get; set; }
        public int sightDis { get; set; }
        public int idelTime { get; set; }
        public int roamTime { get; set; }
        public int maxRoamDis { get; set; }
        public int minRoamDis { get; set; }
        public int roamIdelTime { get; set; }
        public int preAttackTime { get; set; }
        public int attackTime { get; set; }
        public bool initiativeAttack { get; set; }
        public bool counterAttack { get; set; }


        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            moveSpeed = (float)Convert.ToDouble(node.GetAttribute("moveSpeed"));
            sightDis = Convert.ToInt32(node.GetAttribute("sightDis"));
            idelTime = Convert.ToInt32(node.GetAttribute("idelTime"));
            roamTime = Convert.ToInt32(node.GetAttribute("roamTime"));
            maxRoamDis = Convert.ToInt32(node.GetAttribute("maxRoamDis"));
            minRoamDis = Convert.ToInt32(node.GetAttribute("minRoamDis"));
            roamIdelTime = Convert.ToInt32(node.GetAttribute("roamIdelTime"));
            preAttackTime = Convert.ToInt32(node.GetAttribute("preAttackTime"));
            attackTime = Convert.ToInt32(node.GetAttribute("attackTime"));
            initiativeAttack = Convert.ToBoolean(node.GetAttribute("initiativeAttack"));
            counterAttack = Convert.ToBoolean(node.GetAttribute("counterAttack"));
        }

        public IData copy()
        {
            return new MonsterAIData();
        }
    }
}
