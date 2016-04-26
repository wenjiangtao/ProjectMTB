using UnityEngine;
using System.Collections;
namespace MTB
{
    public class SystemPadComponent : PadComponent
    {
        protected override void addPadScript(GameObject pad, int id)
        {
            switch (id)
            {
                case 1:
                    {
                        pad.AddComponent<SystemPad>();
                        pad.GetComponent<SystemPad>().Init(id.ToString(), _type);
                        pad.GetComponent<SystemPad>().setSelect(true);
                        break;
                    }
                case 2:
                    {
                        pad.AddComponent<ControlPad>();
                        pad.GetComponent<ControlPad>().Init(id.ToString(), _type);
                        break;
                    }
                case 3:
                    {
                        pad.AddComponent<ViewPad>();
                        pad.GetComponent<ViewPad>().Init(id.ToString(), _type);
                        break;
                    }
                case 4:
                    {
                        break;
                    }
            }
        }
    }
}
