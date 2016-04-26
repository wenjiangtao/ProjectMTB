using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class MTBArrowData : IData
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<StepArrowData> ArrowList { get; set; }

        public MTBArrowData()
        {
            ArrowList = new List<StepArrowData>();
        }

        public void decode(XmlElement node)
        {
            id = node.GetAttribute("id");
            name = node.GetAttribute("name");
            XmlNodeList nodeList = node.GetElementsByTagName("Step");
            ArrowList = new List<StepArrowData>();
            foreach (XmlElement item in nodeList)
            {
                ArrowList.Add(new StepArrowData(item));
            }
        }

        public XmlElement save(XmlDocument xml)
        {
            XmlElement ArrowDataNode = xml.CreateElement("ArrowData");
            ArrowDataNode.SetAttribute("id", id.ToString());
            ArrowDataNode.SetAttribute("name", name);
            foreach (StepArrowData arrow in ArrowList)
            {
                ArrowDataNode.AppendChild(arrow.saveStep(xml));
            }
            return ArrowDataNode;
        }

        public IData copy()
        {
            return new MTBArrowData();
        }
    }

    public class StepArrowData
    {
        public Vector3 position { get; set; }
        public float rotation { get; set; }

        public StepArrowData(XmlElement node)
        {
            position = new Vector3(Convert.ToSingle(node.GetAttribute("x")), Convert.ToSingle(node.GetAttribute("y")), Convert.ToSingle(node.GetAttribute("z")));
            rotation = Convert.ToSingle(node.GetAttribute("r"));
        }

        public StepArrowData() { }

        public XmlElement saveStep(XmlDocument xml)
        {
            XmlElement element = xml.CreateElement("Step");
            element.SetAttribute("x", position.x.ToString());
            element.SetAttribute("y", position.y.ToString());
            element.SetAttribute("z", position.z.ToString());
            element.SetAttribute("r", rotation.ToString());
            return element;
        }
    }
}
