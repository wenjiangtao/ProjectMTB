using UnityEngine;
using System.Collections;
using System.Xml;
namespace MTB
{
    public interface IData
    {
        void decode(XmlElement node);
        IData copy();
    }
}
