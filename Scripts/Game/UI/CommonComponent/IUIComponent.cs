using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IUIComponent
    {
        void initComponents(params object[] paras);
        void dispose();
    }
}
