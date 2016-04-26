using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
    public class CameraMoveData : IData
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<CameraMoveStep> steps { get; set; }
        public CameraStartPos startpos { get; set; }

        public void decode(XmlElement node)
        {
            id = Convert.ToInt32(node.GetAttribute("id"));
            name = Convert.ToString(node.GetAttribute("name"));
            XmlNodeList nodeList = node.GetElementsByTagName("Step");
            steps = new List<CameraMoveStep>();
            foreach (XmlElement item in nodeList)
            {
                steps.Add(new CameraMoveStep(item));
            }
            startpos = new CameraStartPos(node.GetElementsByTagName("Start")[0] as XmlElement);
        }

        public XmlElement save(XmlDocument xml)
        {
            XmlElement pathDateNode = xml.CreateElement("PathData");
            pathDateNode.SetAttribute("id", id.ToString());
            pathDateNode.SetAttribute("name", name);
            pathDateNode.AppendChild(startpos.saveStep(xml));
            foreach (CameraMoveStep step in steps)
            {
                pathDateNode.AppendChild(step.saveStep(xml));
            }
            return pathDateNode;
        }

        public IData copy()
        {
            return new CameraMoveData();
        }
    }

    public class CameraMoveStep
    {
        public int id { get; set; }
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public float time { get; set; }

        public CameraMoveStep() { }

        public CameraMoveStep(XmlElement element)
        {
            id = Convert.ToInt32(element.GetAttribute("id"));
            float x = Convert.ToSingle(element.GetAttribute("x"));
            float y = Convert.ToSingle(element.GetAttribute("y"));
            float z = Convert.ToSingle(element.GetAttribute("z"));
            position = new Vector3(x, y, z);
            float rx = Convert.ToSingle(element.GetAttribute("rx"));
            float ry = Convert.ToSingle(element.GetAttribute("ry"));
            float rz = Convert.ToSingle(element.GetAttribute("rz"));
            rotation = new Vector3(rx, ry, rz);
            time = Convert.ToInt32(element.GetAttribute("time"));
        }

        public XmlElement saveStep(XmlDocument xml)
        {
            XmlElement element = xml.CreateElement("Step");

            element.SetAttribute("id", id.ToString());
            element.SetAttribute("x", position.x.ToString());
            element.SetAttribute("y", position.y.ToString());
            element.SetAttribute("z", position.z.ToString());

            element.SetAttribute("rx", rotation.x.ToString());
            element.SetAttribute("ry", rotation.y.ToString());
            element.SetAttribute("rz", rotation.z.ToString());

            element.SetAttribute("time", time.ToString());
            return element;
        }
    }

    public class CameraStartPos
    {
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }

        public CameraStartPos() { }

        public CameraStartPos(XmlElement element)
        {
            float x = Convert.ToSingle(element.GetAttribute("x"));
            float y = Convert.ToSingle(element.GetAttribute("y"));
            float z = Convert.ToSingle(element.GetAttribute("z"));
            position = new Vector3(x, y, z);
            float rx = Convert.ToSingle(element.GetAttribute("rx"));
            float ry = Convert.ToSingle(element.GetAttribute("ry"));
            float rz = Convert.ToSingle(element.GetAttribute("rz"));
            rotation = new Vector3(rx, ry, rz);
        }

        public XmlElement saveStep(XmlDocument xml)
        {
            XmlElement element = xml.CreateElement("Start");

            element.SetAttribute("x", position.x.ToString());
            element.SetAttribute("y", position.y.ToString());
            element.SetAttribute("z", position.z.ToString());

            element.SetAttribute("rx", rotation.x.ToString());
            element.SetAttribute("ry", rotation.y.ToString());
            element.SetAttribute("rz", rotation.z.ToString());
            return element;
        }
    }
}
