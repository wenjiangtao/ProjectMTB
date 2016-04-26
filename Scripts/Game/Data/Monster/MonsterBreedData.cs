using UnityEngine;
using System;
using System.Collections;
using System.Xml;
namespace MTB
{
    public class MonsterBreedData : IData
    {
        public int id { get; set; }
        public int decoyItem { get; set; }
        public int decoyDis { get; set; }
        public int feedTime { get; set; }
        public int breedType { get; set; }
        public int breedItem { get; set; }
        public int breedDis { get; set; }
        public float breedRate { get; set; }
        public int growTime { get; set; }
        public int growItem { get; set; }
        public int growEffect { get; set; }
        public int cubID { get; set; }
        public int adultID { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            decoyItem = Convert.ToInt32(node.GetAttribute("decoyItem"));
            decoyDis = Convert.ToInt32(node.GetAttribute("decoyDis"));
            feedTime = Convert.ToInt32(node.GetAttribute("feedTime"));
            breedType = Convert.ToInt32(node.GetAttribute("breedType"));
            breedItem = Convert.ToInt32(node.GetAttribute("breedItem"));
            breedDis = Convert.ToInt32(node.GetAttribute("breedDis"));
            breedRate = (float)Convert.ToDouble(node.GetAttribute("breedRate"));
            growTime = Convert.ToInt32(node.GetAttribute("growTime"));
            growItem = Convert.ToInt32(node.GetAttribute("growItem"));
            growEffect = Convert.ToInt32(node.GetAttribute("growEffect"));
            cubID = Convert.ToInt32(node.GetAttribute("cubID"));
            adultID = Convert.ToInt32(node.GetAttribute("adultID"));
        }

        public IData copy()
        {
            return new MonsterBreedData();
        }
    }
}
