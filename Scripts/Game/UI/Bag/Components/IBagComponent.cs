
using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IBagComponent
    {
        void initComponents(params object[] paras);
        void refresh();
        void dispose();
    }
}
