using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class MTBPlantData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int decorationType { get; set; }
        public int nextId { get; set; }
        public int growTime { get; set; }
        public int seedId { get; set; }

        public int[] chunkWidth { get; set; }
        public int[] chunkHeight { get; set; }
        public int[] leafWidth { get; set; }
        public int[] leafHeight { get; set; }
        public int leafOffset { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = Convert.ToString(node.GetAttribute("name"));
            decorationType = Convert.ToInt32(node.GetAttribute("decorationType"));
            nextId = Convert.ToInt32(node.GetAttribute("nextId"));
            growTime = Convert.ToInt32(node.GetAttribute("growTime"));
            seedId = Convert.ToInt32(node.GetAttribute("seedId"));

            string[] cw = Convert.ToString(node.GetAttribute("chunkWidth")).Split(',');
            chunkWidth = new int[cw.Length];
            for (int i = 0; i < cw.Length; i++)
            {
                chunkWidth[i] = Convert.ToInt32(cw[i]);
            }

            string[] ch = Convert.ToString(node.GetAttribute("chunkHeight")).Split(',');
            chunkHeight = new int[ch.Length];
            for (int i = 0; i < ch.Length; i++)
            {
                chunkHeight[i] = Convert.ToInt32(ch[i]);
            }

            string[] lw = Convert.ToString(node.GetAttribute("leafWidth")).Split(',');
            leafWidth = new int[lw.Length];
            for (int i = 0; i < lw.Length; i++)
            {
                leafWidth[i] = Convert.ToInt32(lw[i]);
            }

            string[] lh = Convert.ToString(node.GetAttribute("leafHeight")).Split(',');
            leafHeight = new int[lh.Length];
            for (int i = 0; i < lh.Length; i++)
            {
                leafHeight[i] = Convert.ToInt32(lh[i]);
            }

            leafOffset = -1 * Convert.ToInt32(node.GetAttribute("leafOffset"));
        }
        public IData copy()
        {
            return new MTBTaskData();
        }
    }
}
