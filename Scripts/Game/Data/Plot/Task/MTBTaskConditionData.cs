using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class MTBTaskConditionData : IData
    {
        public string id { get; set; }
        public StartTriggerData startTriggerCondition { get; set; }
        public FinishTriggerData finishTriggerCondition { get; set; }
        public FinishData finishCondition { get; set; }
        public TipsData tipsCondition { get; set; }

        public void decode(XmlElement node)
        {
            id = node.GetAttribute("id");
            XmlElement startTrigger = node.GetElementsByTagName("startTriggerCondition")[0] as XmlElement;
            XmlElement finishTrigger = node.GetElementsByTagName("finishTriggerCondition")[0] as XmlElement;
            XmlElement finish = node.GetElementsByTagName("finishCondition")[0] as XmlElement;
            XmlElement tip = node.GetElementsByTagName("Tip")[0] as XmlElement;
            startTriggerCondition = new StartTriggerData(startTrigger);
            finishTriggerCondition = new FinishTriggerData(finishTrigger);
            finishCondition = new FinishData(finish);
            tipsCondition = new TipsData(tip);
        }

        public XmlElement save(XmlDocument xml)
        {
            XmlElement posNode = xml.CreateElement("Condition");
            posNode.SetAttribute("id", id);

            XmlNode root = xml.GetElementById(id);
            XmlElement element1 = xml.CreateElement("startTriggerCondition");
            element1.SetAttribute("name", startTriggerCondition.scriptName);
            root.AppendChild(element1);

            XmlElement element2 = xml.CreateElement("finishTriggerCondition");
            element2.SetAttribute("name", finishTriggerCondition.scriptName);
            root.AppendChild(element2);

            XmlElement element3 = xml.CreateElement("finishCondition");
            element3.SetAttribute("name", finishCondition.scriptName);
            root.AppendChild(element3);

            return posNode;
        }

        public IData copy()
        {
            return new MTBTaskConditionData();
        }
    }

    public class StartTriggerData
    {
        public string scriptName { get; set; }
        public Dictionary<string, string> paras = new Dictionary<string, string>();

        public StartTriggerData(XmlElement node)
        {
            scriptName = ExcelParamsToScript.startTriggerMap[node.GetAttribute("name")];
            IEnumerator enumrator = node.Attributes.GetEnumerator();
            while (enumrator.MoveNext())
            {
                XmlAttribute attr = enumrator.Current as XmlAttribute;
                paras.Add(attr.Name, attr.Value);
            }
        }
    }

    public class FinishTriggerData
    {
        public string scriptName { get; set; }
        public Dictionary<string, string> paras = new Dictionary<string, string>();

        public FinishTriggerData(XmlElement node)
        {
            scriptName = ExcelParamsToScript.finishTriggerMap[node.GetAttribute("name")];
            IEnumerator enumrator = node.Attributes.GetEnumerator();
            while (enumrator.MoveNext())
            {
                XmlAttribute attr = enumrator.Current as XmlAttribute;
                if (ExcelParamsToScript.actionMap.ContainsKey(attr.Value))
                    attr.Value = ExcelParamsToScript.actionMap[attr.Value];
                paras.Add(attr.Name, attr.Value);
            }
        }
    }

    public class FinishData
    {
        public string scriptName { get; set; }
        public Dictionary<string, string> paras = new Dictionary<string, string>();

        public FinishData(XmlElement node)
        {
            scriptName = ExcelParamsToScript.finishMap[node.GetAttribute("name")];
            IEnumerator enumrator = node.Attributes.GetEnumerator();
            while (enumrator.MoveNext())
            {
                XmlAttribute attr = enumrator.Current as XmlAttribute;
                paras.Add(attr.Name, attr.Value);
            }
        }
    }

    public class TipsData
    {
        public string content { get; set; }
        public Dictionary<string, string> paras = new Dictionary<string, string>();

        public TipsData(XmlElement node)
        {
            content = node.GetAttribute("content");
            IEnumerator enumrator = node.Attributes.GetEnumerator();
            while (enumrator.MoveNext())
            {
                XmlAttribute attr = enumrator.Current as XmlAttribute;
                paras.Add(attr.Name, attr.Value);
            }
        }
    }

    public class ExcelParamsToScript
    {
        public static Dictionary<string, string> startTriggerMap = new Dictionary<string, string>()
        {
            {"NPC","DistanceStartCondition"},
            {"自动","AutoStartCondition"}
        };

        public static Dictionary<string, string> finishTriggerMap = new Dictionary<string, string>(){
           {"对话","DialogFinishTriggerCondition"},
           {"相机","CameraMoveFinishTriggerCondition"},
           {"物品","QuantityFinishTriggerCondition"},
           {"动作","ActionFinishTriggerCondition"},
           {"自动","AutoFinishTriggerCondition"}
        };

        public static Dictionary<string, string> finishMap = new Dictionary<string, string>()
        {
            {"自动","AutoFinishCondition"},
            {"物品","QuantityFinishCondition"},
        };

        public static Dictionary<string, string> actionMap = new Dictionary<string, string>(){
          {"点击NPC","ScreenInteractNPCActionScript"}
        };
    }
}
