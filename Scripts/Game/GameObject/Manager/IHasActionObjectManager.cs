using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public interface IHasActionObjectManager
    {
        List<GameObject> listObj();

        void removeObj(GameObject obj);

        void removeObj(int aoId);

        GameObject getObjById(int id);

        void objectDead(GameObject obj);

        void setObjectNameVisible(bool b);

        GameObject[] getOppositeGroupObj(int groupId);

        void dispose();
    }
}
